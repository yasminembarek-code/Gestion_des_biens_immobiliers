namespace GestionBiensImmobiliers
{
    public class DemandeMaintenance
    {
        private static int _compteur = 0;

        public int Id { get; }
        public Bien Bien { get; }
        public string Description { get; }
        public Priorite Priorite { get; }
        public DateTime DateDemande { get; }
        public StatutMaintenance Statut { get; private set; } = StatutMaintenance.Ouverte;
        public Technicien Technicien { get; private set; }
        public decimal Cout { get; private set; }
        public DateTime? DateCloture { get; private set; }

        public DemandeMaintenance(Bien bien, string description,
                                  Priorite priorite = Priorite.Normale, DateTime? dateDemande = null)
        {
            Id = ++_compteur;
            Bien = bien;
            Description = description;
            Priorite = priorite;
            DateDemande = (dateDemande ?? DateTime.Today).Date;
            bien.AjouterDemandeMaintenance(this);
        }

        public void Assigner(Technicien technicien)
        {
            Technicien = technicien;
            technicien.Interventions.Add(this);
        }

        public void Demarrer()
        {
            if (Technicien == null)
                throw new InvalidOperationException("Assignez un technicien avant de démarrer.");
            Statut = StatutMaintenance.EnCours;
        }

        public void Terminer(decimal cout, DateTime? dateCloture = null)
        {
            if (cout < 0)
                throw new ArgumentException("Le coût ne peut pas être négatif.");
            Cout = cout;
            DateCloture = (dateCloture ?? DateTime.Today).Date;
            Statut = StatutMaintenance.Terminee;
        }

        public void Annuler() => Statut = StatutMaintenance.Annulee;

        public override string ToString()
            => $"Maintenance #{Id} [{Priorite}] {Description} - {Statut} ({Cout})";
    }
}
