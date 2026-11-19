namespace GestionBiensImmobiliers
{
    /// <summary>Point d'entrée du système : façade + statistiques.</summary>
    public class GestionImmobiliere
    {
        public string NomAgence { get; }
        public List<Proprietaire> Proprietaires { get; } = new List<Proprietaire>();
        public List<Locataire> Locataires { get; } = new List<Locataire>();
        public List<Technicien> Techniciens { get; } = new List<Technicien>();
        public List<Bien> Biens { get; } = new List<Bien>();
        public List<Contrat> Contrats { get; } = new List<Contrat>();
        public List<DemandeMaintenance> Demandes { get; } = new List<DemandeMaintenance>();

        public GestionImmobiliere(string nomAgence) => NomAgence = nomAgence;

        // --- Enregistrement ---
        public Proprietaire AjouterProprietaire(Proprietaire p) { Proprietaires.Add(p); return p; }
        public Locataire AjouterLocataire(Locataire l) { Locataires.Add(l); return l; }
        public Technicien AjouterTechnicien(Technicien t) { Techniciens.Add(t); return t; }

        public Bien AjouterBien(Bien bien, Proprietaire proprietaire)
        {
            proprietaire.AjouterBien(bien);
            Biens.Add(bien);
            return bien;
        }

        // --- Opérations ---
        public Contrat CreerContrat(Bien bien, Locataire locataire, DateTime debut, DateTime fin,
                                    decimal depotGarantie = 0)
        {
            var contrat = new Contrat(bien, locataire, debut, fin, depotGarantie);
            contrat.GenererEcheancier();
            Contrats.Add(contrat);
            return contrat;
        }

        public DemandeMaintenance SignalerPanne(Bien bien, string description, Priorite priorite)
        {
            var demande = new DemandeMaintenance(bien, description, priorite);
            Demandes.Add(demande);
            return demande;
        }

        public List<Bien> BiensDisponibles() => Biens.Where(b => b.EstDisponible()).ToList();

        // --- Statistiques / tableau de bord ---
        public double TauxOccupation()
        {
            if (Biens.Count == 0) return 0.0;
            int loues = Biens.Count(b => b.Statut == StatutBien.Loue);
            return Math.Round(100.0 * loues / Biens.Count, 1);
        }

        public List<Loyer> LoyersImpayes(DateTime? aujourdhui = null)
            => Contrats.SelectMany(c => c.LoyersImpayes(aujourdhui)).ToList();

        public List<Contrat> ContratsAEcheance(int jours = 60, DateTime? aujourdhui = null)
        {
            DateTime jour = (aujourdhui ?? DateTime.Today).Date;
            DateTime limite = jour.AddDays(jours);
            return Contrats.Where(c => c.EstActif(jour) && c.DateFin <= limite).ToList();
        }

        public decimal RevenusEncaisses()
            => Contrats.SelectMany(c => c.Loyers).Sum(l => l.MontantPaye);

        public decimal RevenusAttendus(DateTime? jusqua = null)
        {
            DateTime limite = (jusqua ?? DateTime.Today).Date;
            return Contrats.SelectMany(c => c.Loyers)
                           .Where(l => l.DateEcheance <= limite)
                           .Sum(l => l.Montant);
        }

        public decimal DepensesMaintenance()
            => Demandes.Where(d => d.Statut == StatutMaintenance.Terminee).Sum(d => d.Cout);

        public Dictionary<string, object> TableauDeBord(DateTime? aujourdhui = null)
        {
            return new Dictionary<string, object>
            {
                ["taux_occupation_%"] = TauxOccupation(),
                ["loyers_impayes"] = LoyersImpayes(aujourdhui).Count,
                ["contrats_bientot_termines"] = ContratsAEcheance(60, aujourdhui).Count,
                ["revenus_encaisses"] = RevenusEncaisses(),
                ["depenses_maintenance"] = DepensesMaintenance()
            };
        }
    }
}
