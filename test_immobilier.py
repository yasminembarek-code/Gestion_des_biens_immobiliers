import unittest
from datetime import date

from enums import StatutBien, StatutLoyer, Priorite
from personnes import Proprietaire, Locataire, Technicien
from biens import Appartement
from contrat import Contrat
from gestion import GestionImmobiliere


def fabrique():
    prop = Proprietaire("A", "B", "a@b.c", "1")
    loc = Locataire("C", "D", "c@d.e", "2")
    bien = Appartement("1 rue X", "Tunis", 50, 500)
    prop.ajouter_bien(bien)
    return prop, loc, bien


class TestImmobilier(unittest.TestCase):
    def test_contrat_rend_bien_loue(self):
        _, loc, bien = fabrique()
        Contrat(bien, loc, date(2026, 1, 1), date(2026, 12, 31))
        self.assertEqual(bien.statut, StatutBien.LOUE)

    def test_bien_deja_loue_refuse(self):
        _, loc, bien = fabrique()
        Contrat(bien, loc, date(2026, 1, 1), date(2026, 12, 31))
        with self.assertRaises(ValueError):
            Contrat(bien, loc, date(2026, 1, 1), date(2026, 12, 31))

    def test_echeancier_12_mois(self):
        _, loc, bien = fabrique()
        c = Contrat(bien, loc, date(2026, 1, 1), date(2026, 12, 31))
        self.assertEqual(len(c.generer_echeancier()), 12)

    def test_paiement_partiel_puis_total(self):
        _, loc, bien = fabrique()
        c = Contrat(bien, loc, date(2026, 1, 1), date(2026, 3, 31))
        loyer = c.generer_echeancier()[0]
        loyer.enregistrer_paiement(200, date(2026, 1, 2))
        self.assertEqual(loyer.statut, StatutLoyer.PARTIEL)
        loyer.enregistrer_paiement(300, date(2026, 1, 3))
        self.assertEqual(loyer.statut, StatutLoyer.PAYE)
        self.assertIn("QUITTANCE", loyer.generer_quittance().texte())

    def test_retard_et_penalite(self):
        _, loc, bien = fabrique()
        c = Contrat(bien, loc, date(2026, 1, 1), date(2026, 3, 31))
        loyer = c.generer_echeancier()[0]
        self.assertEqual(loyer.penalite(date(2026, 1, 20)), 25.0)

    def test_taux_occupation(self):
        agence = GestionImmobiliere("T")
        prop, loc, bien = fabrique()
        agence.ajouter_bien(bien, prop)
        self.assertEqual(agence.taux_occupation(), 0.0)
        agence.creer_contrat(bien, loc, date(2026, 1, 1), date(2026, 12, 31))
        self.assertEqual(agence.taux_occupation(), 100.0)

    def test_maintenance(self):
        agence = GestionImmobiliere("T")
        prop, _, bien = fabrique()
        agence.ajouter_bien(bien, prop)
        d = agence.signaler_panne(bien, "Fuite", Priorite.HAUTE)
        d.assigner(Technicien("E", "F", "e@f.g", "3", "Plomberie"))
        d.demarrer()
        d.terminer(80)
        self.assertEqual(bien.cout_maintenance_total(), 80)


if __name__ == "__main__":
    unittest.main()
