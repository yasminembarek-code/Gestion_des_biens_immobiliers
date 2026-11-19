namespace GestionBiensImmobiliers
{
    public class Contrat
    {
        public const int JourEcheance = 5;
        private static int _compteur = 0;

        public int Id { get; }
        public Bien Bien { get; }
        public Locataire Locataire { get; }
        public DateTime DateDebut { get; }
        public DateTime DateFin { get; }
        public decimal DepotGarantie { get; }
        public decimal LoyerMensuel { get; private set; }
        public StatutContrat Statut { get; private set; } = StatutContrat.Actif;
        public List<Loyer> Loyers { get; private set; } = new List<Loyer>();

        public Contrat(Bien bien, Locataire locataire, DateTime dateDebut, DateTime dateFin,
                       decimal depotGarantie = 0, decimal? loyerMensuel = null)
        {
            if (dateFin <= dateDebut)
                throw new ArgumentException("La date de fin doit suivre la date de début.");
            if (!bien.EstDisponible())
                throw new InvalidOperationException("Ce bien n'est pas disponible à la location.");

            Id = ++_compteur;
            Bien = bien;
            Locataire = locataire;
            DateDebut = dateDebut.Date;
            DateFin = dateFin.Date;
            DepotGarantie = depotGarantie;
            LoyerMensuel = loyerMensuel ?? bien.LoyerBase;

            bien.Contrats.Add(this);
            bien.ChangerStatut(StatutBien.Loue);
            locataire.Contrats.Add(this);
        }

        public bool EstActif(DateTime? aujourdhui = null)
        {
            DateTime jour = (aujourdhui ?? DateTime.Today).Date;
            return Statut == StatutContrat.Actif && DateDebut <= jour && jour <= DateFin;
        }

        public int JoursRestants(DateTime? aujourdhui = null)
            => (DateFin - (aujourdhui ?? DateTime.Today).Date).Days;

        /// <summary>Crée un loyer par mois entre la date de début et la date de fin.</summary>
        public List<Loyer> GenererEcheancier()
        {
            Loyers = new List<Loyer>();
            var mois = new DateTime(DateDebut.Year, DateDebut.Month, 1);
            while (mois <= DateFin)
            {
                int jour = Math.Min(JourEcheance, DateTime.DaysInMonth(mois.Year, mois.Month));
                var echeance = new DateTime(mois.Year, mois.Month, jour);
                Loyers.Add(new Loyer(this, mois, LoyerMensuel, echeance));
                mois = mois.AddMonths(1);
            }
            return Loyers;
        }

        /// <summary>Révision annuelle : applique un taux (0.02 = +2 %) aux loyers non payés.</summary>
        public decimal ReviserLoyer(decimal taux)
        {
            LoyerMensuel = Math.Round(LoyerMensuel * (1 + taux), 2);
            foreach (var loyer in Loyers.Where(l => l.MontantPaye == 0))
                loyer.Montant = LoyerMensuel;
            return LoyerMensuel;
        }

        public List<Loyer> LoyersImpayes(DateTime? aujourdhui = null)
            => Loyers.Where(l => l.MettreAJourStatut(aujourdhui) == StatutLoyer.EnRetard).ToList();

        public void Resilier()
        {
            Statut = StatutContrat.Resilie;
            Bien.ChangerStatut(StatutBien.Disponible);
        }

        public void Terminer()
        {
            Statut = StatutContrat.Termine;
            Bien.ChangerStatut(StatutBien.Disponible);
        }

        public override string ToString()
            => $"Contrat #{Id}: {Locataire.NomComplet} / {Bien.Adresse} " +
               $"({DateDebut:yyyy-MM-dd} -> {DateFin:yyyy-MM-dd}) [{Statut}]";
    }
}
