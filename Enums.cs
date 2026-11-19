namespace GestionBiensImmobiliers
{
    public enum StatutBien { Disponible, Loue, EnTravaux }

    public enum StatutContrat { Actif, Termine, Resilie }

    public enum StatutLoyer { EnAttente, Partiel, Paye, EnRetard }

    public enum ModePaiement { Especes, Virement, Cheque, Carte }

    public enum Priorite { Basse = 1, Normale = 2, Haute = 3, Urgente = 4 }

    public enum StatutMaintenance { Ouverte, EnCours, Terminee, Annulee }
}
