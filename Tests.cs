using GestionBiensImmobiliers;

/// <summary>Tests unitaires simples (sans bibliothèque externe).</summary>
public static class Tests
{
    private static int _reussis = 0;
    private static int _echoues = 0;

    private static void Verifier(string nom, bool condition)
    {
        if (condition) { _reussis++; Console.WriteLine($"  OK    {nom}"); }
        else { _echoues++; Console.WriteLine($"  ECHEC {nom}"); }
    }

    private static bool Leve<T>(Action action) where T : Exception
    {
        try { action(); return false; }
        catch (T) { return true; }
    }

    private static (Proprietaire, Locataire, Bien) Fabrique()
    {
        var prop = new Proprietaire("A", "B", "a@b.c", "1");
        var loc = new Locataire("C", "D", "c@d.e", "2");
        Bien bien = new Appartement("1 rue X", "Tunis", 50, 500);
        prop.AjouterBien(bien);
        return (prop, loc, bien);
    }

    public static int Lancer()
    {
        Console.WriteLine("Tests :");

        {
            var (_, loc, bien) = Fabrique();
            new Contrat(bien, loc, new DateTime(2026, 1, 1), new DateTime(2026, 12, 31));
            Verifier("contrat rend le bien loué", bien.Statut == StatutBien.Loue);
            Verifier("bien déjà loué refusé", Leve<InvalidOperationException>(
                () => new Contrat(bien, loc, new DateTime(2026, 1, 1), new DateTime(2026, 12, 31))));
        }
        {
            var (_, loc, bien) = Fabrique();
            var c = new Contrat(bien, loc, new DateTime(2026, 1, 1), new DateTime(2026, 12, 31));
            Verifier("échéancier de 12 mois", c.GenererEcheancier().Count == 12);
        }
        {
            var (_, loc, bien) = Fabrique();
            var c = new Contrat(bien, loc, new DateTime(2026, 1, 1), new DateTime(2026, 3, 31));
            var loyer = c.GenererEcheancier()[0];
            loyer.EnregistrerPaiement(200, new DateTime(2026, 1, 2));
            Verifier("paiement partiel", loyer.Statut == StatutLoyer.Partiel);
            loyer.EnregistrerPaiement(300, new DateTime(2026, 1, 3));
            Verifier("paiement total", loyer.Statut == StatutLoyer.Paye);
            Verifier("quittance émise", loyer.GenererQuittance().Texte().Contains("QUITTANCE"));
        }
        {
            var (_, loc, bien) = Fabrique();
            var c = new Contrat(bien, loc, new DateTime(2026, 1, 1), new DateTime(2026, 3, 31));
            var loyer = c.GenererEcheancier()[0];
            Verifier("pénalité de retard (5 %)", loyer.Penalite(new DateTime(2026, 1, 20)) == 25.00m);
        }
        {
            var agence = new GestionImmobiliere("T");
            var (prop, loc, bien) = Fabrique();
            agence.AjouterBien(bien, prop);
            Verifier("taux d'occupation 0 %", agence.TauxOccupation() == 0.0);
            agence.CreerContrat(bien, loc, new DateTime(2026, 1, 1), new DateTime(2026, 12, 31));
            Verifier("taux d'occupation 100 %", agence.TauxOccupation() == 100.0);
        }
        {
            var agence = new GestionImmobiliere("T");
            var (prop, _, bien) = Fabrique();
            agence.AjouterBien(bien, prop);
            var d = agence.SignalerPanne(bien, "Fuite", Priorite.Haute);
            d.Assigner(new Technicien("E", "F", "e@f.g", "3", "Plomberie"));
            d.Demarrer();
            d.Terminer(80);
            Verifier("coût de maintenance du bien", bien.CoutMaintenanceTotal() == 80m);
        }

        Console.WriteLine($"\n{_reussis} réussis, {_echoues} échoués");
        return _echoues == 0 ? 0 : 1;
    }
}
