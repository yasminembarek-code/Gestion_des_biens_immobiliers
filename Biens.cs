namespace GestionBiensImmobiliers
{
    /// <summary>Classe abstraite : un bien immobilier.</summary>
    public abstract class Bien
    {
        private static int _compteur = 0;

        public int Id { get; }
        public string Adresse { get; set; }
        public string Ville { get; set; }
        public double Surface { get; set; }
        public decimal LoyerBase { get; set; }
        public StatutBien Statut { get; private set; } = StatutBien.Disponible;
        public Proprietaire Proprietaire { get; set; }
        public List<Contrat> Contrats { get; } = new List<Contrat>();
        public List<DemandeMaintenance> DemandesMaintenance { get; } = new List<DemandeMaintenance>();

        protected Bien(string adresse, string ville, double surface, decimal loyerBase)
        {
            if (surface <= 0 || loyerBase < 0)
                throw new ArgumentException("Surface et loyer doivent être positifs.");
            Id = ++_compteur;
            Adresse = adresse;
            Ville = ville;
            Surface = surface;
            LoyerBase = loyerBase;
        }

        public abstract string TypeBien();

        public bool EstDisponible() => Statut == StatutBien.Disponible;

        public void ChangerStatut(StatutBien statut) => Statut = statut;

        public void AjouterDemandeMaintenance(DemandeMaintenance demande)
            => DemandesMaintenance.Add(demande);

        public decimal CoutMaintenanceTotal() => DemandesMaintenance.Sum(d => d.Cout);

        public decimal LoyerAuM2() => Math.Round(LoyerBase / (decimal)Surface, 2);

        public override string ToString()
            => $"{TypeBien()} #{Id} - {Adresse}, {Ville} ({Surface} m², {LoyerBase} /mois) [{Statut}]";
    }

    public class Appartement : Bien
    {
        public int Etage { get; set; }
        public bool Ascenseur { get; set; }

        public Appartement(string adresse, string ville, double surface, decimal loyerBase,
                           int etage = 0, bool ascenseur = false)
            : base(adresse, ville, surface, loyerBase)
        {
            Etage = etage;
            Ascenseur = ascenseur;
        }

        public override string TypeBien() => "Appartement";
    }

    public class Maison : Bien
    {
        public double SurfaceJardin { get; set; }
        public bool Garage { get; set; }

        public Maison(string adresse, string ville, double surface, decimal loyerBase,
                      double surfaceJardin = 0, bool garage = false)
            : base(adresse, ville, surface, loyerBase)
        {
            SurfaceJardin = surfaceJardin;
            Garage = garage;
        }

        public override string TypeBien() => "Maison";
    }

    public class LocalCommercial : Bien
    {
        public string Activite { get; set; }

        public LocalCommercial(string adresse, string ville, double surface, decimal loyerBase,
                               string activite = "")
            : base(adresse, ville, surface, loyerBase)
        {
            Activite = activite;
        }

        public override string TypeBien() => "Local commercial";
    }
}
