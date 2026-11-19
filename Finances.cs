namespace GestionBiensImmobiliers
{
    public class Paiement
    {
        private static int _compteur = 0;

        public int Id { get; }
        public Loyer Loyer { get; }
        public decimal Montant { get; }
        public DateTime DatePaiement { get; }
        public ModePaiement Mode { get; }

        public Paiement(Loyer loyer, decimal montant, DateTime? datePaiement = null,
                        ModePaiement mode = ModePaiement.Virement)
        {
            if (montant <= 0)
                throw new ArgumentException("Le montant doit être positif.");
            Id = ++_compteur;
            Loyer = loyer;
            Montant = montant;
            DatePaiement = (datePaiement ?? DateTime.Today).Date;
            Mode = mode;
        }

        public override string ToString()
            => $"Paiement #{Id}: {Montant} le {DatePaiement:yyyy-MM-dd} ({Mode})";
    }

    public class Loyer
    {
        public const decimal TauxPenalite = 0.05m;
        private static int _compteur = 0;

        public int Id { get; }
        public Contrat Contrat { get; }
        public DateTime Mois { get; }
        public decimal Montant { get; set; }
        public DateTime DateEcheance { get; }
        public StatutLoyer Statut { get; private set; } = StatutLoyer.EnAttente;
        public List<Paiement> Paiements { get; } = new List<Paiement>();

        public Loyer(Contrat contrat, DateTime mois, decimal montant, DateTime dateEcheance)
        {
            Id = ++_compteur;
            Contrat = contrat;
            Mois = mois;
            Montant = montant;
            DateEcheance = dateEcheance;
        }

        public decimal MontantPaye => Paiements.Sum(p => p.Montant);

        public decimal ResteAPayer => Math.Max(0m, Math.Round(Montant - MontantPaye, 2));

        public Paiement EnregistrerPaiement(decimal montant, DateTime? datePaiement = null,
                                            ModePaiement mode = ModePaiement.Virement)
        {
            if (montant > ResteAPayer)
                throw new InvalidOperationException("Le paiement dépasse le reste à payer.");
            var paiement = new Paiement(this, montant, datePaiement, mode);
            Paiements.Add(paiement);
            MettreAJourStatut(paiement.DatePaiement);
            return paiement;
        }

        public StatutLoyer MettreAJourStatut(DateTime? aujourdhui = null)
        {
            DateTime jour = (aujourdhui ?? DateTime.Today).Date;
            if (ResteAPayer == 0)
                Statut = StatutLoyer.Paye;
            else if (jour > DateEcheance)
                Statut = StatutLoyer.EnRetard;
            else if (MontantPaye > 0)
                Statut = StatutLoyer.Partiel;
            else
                Statut = StatutLoyer.EnAttente;
            return Statut;
        }

        public decimal Penalite(DateTime? aujourdhui = null)
        {
            if (MettreAJourStatut(aujourdhui) == StatutLoyer.EnRetard)
                return Math.Round(ResteAPayer * TauxPenalite, 2);
            return 0m;
        }

        public Quittance GenererQuittance()
        {
            if (Statut != StatutLoyer.Paye)
                throw new InvalidOperationException("Une quittance n'est émise que pour un loyer soldé.");
            return new Quittance(this);
        }

        public override string ToString()
            => $"Loyer #{Id} {Mois:MM/yyyy}: {Montant} [{Statut}] reste {ResteAPayer}";
    }

    public class Quittance
    {
        private static int _compteur = 0;

        public int Id { get; }
        public Loyer Loyer { get; }
        public DateTime DateEmission { get; }

        public Quittance(Loyer loyer, DateTime? dateEmission = null)
        {
            Id = ++_compteur;
            Loyer = loyer;
            DateEmission = (dateEmission ?? DateTime.Today).Date;
        }

        public string Texte()
        {
            var c = Loyer.Contrat;
            return $"QUITTANCE DE LOYER n°{Id}\n" +
                   $"Locataire : {c.Locataire.NomComplet}\n" +
                   $"Bien : {c.Bien.Adresse}, {c.Bien.Ville}\n" +
                   $"Période : {Loyer.Mois:MM/yyyy}\n" +
                   $"Montant réglé : {Loyer.Montant}\n" +
                   $"Émise le : {DateEmission:yyyy-MM-dd}";
        }
    }
}
