namespace GestionBiensImmobiliers
{
    /// <summary>Classe abstraite : une personne du système.</summary>
    public abstract class Personne
    {
        private static int _compteur = 0;

        public int Id { get; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }

        protected Personne(string nom, string prenom, string email, string telephone)
        {
            Id = ++_compteur;
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Telephone = telephone;
        }

        public string NomComplet => $"{Prenom} {Nom}";

        public abstract string Role();

        public override string ToString() => $"{Role()} #{Id} - {NomComplet}";
    }

    public class Proprietaire : Personne
    {
        public List<Bien> Biens { get; } = new List<Bien>();

        public Proprietaire(string nom, string prenom, string email, string telephone)
            : base(nom, prenom, email, telephone) { }

        public override string Role() => "Propriétaire";

        public void AjouterBien(Bien bien)
        {
            if (!Biens.Contains(bien))
            {
                Biens.Add(bien);
                bien.Proprietaire = this;
            }
        }

        public decimal RevenusPotentiels() => Biens.Sum(b => b.LoyerBase);
    }

    public class Locataire : Personne
    {
        public string Profession { get; set; }
        public List<Contrat> Contrats { get; } = new List<Contrat>();

        public Locataire(string nom, string prenom, string email, string telephone, string profession = "")
            : base(nom, prenom, email, telephone)
        {
            Profession = profession;
        }

        public override string Role() => "Locataire";

        public List<Contrat> ContratsActifs(DateTime? aujourdhui = null)
            => Contrats.Where(c => c.EstActif(aujourdhui)).ToList();
    }

    public class Technicien : Personne
    {
        public string Specialite { get; set; }
        public List<DemandeMaintenance> Interventions { get; } = new List<DemandeMaintenance>();

        public Technicien(string nom, string prenom, string email, string telephone, string specialite)
            : base(nom, prenom, email, telephone)
        {
            Specialite = specialite;
        }

        public override string Role() => $"Technicien ({Specialite})";

        public int ChargeDeTravail()
            => Interventions.Count(d => d.Statut == StatutMaintenance.EnCours);
    }
}
