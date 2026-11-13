"""Démonstration du système de gestion des biens immobiliers."""
from datetime import date

from enums import Priorite, ModePaiement
from personnes import Proprietaire, Locataire, Technicien
from biens import Appartement, Maison, LocalCommercial
from gestion import GestionImmobiliere


def main():
    agence = GestionImmobiliere("Agence Médina Immo")

    prop = agence.ajouter_proprietaire(Proprietaire("Ben Ali", "Karim", "karim@mail.tn", "20111222"))
    loc1 = agence.ajouter_locataire(Locataire("Trabelsi", "Sana", "sana@mail.tn", "55123456", "Ingénieure"))
    loc2 = agence.ajouter_locataire(Locataire("Gharbi", "Omar", "omar@mail.tn", "98765432", "Commerçant"))
    tech = agence.ajouter_technicien(Technicien("Jlassi", "Mehdi", "mehdi@mail.tn", "22334455", "Plomberie"))

    appart = agence.ajouter_bien(Appartement("12 rue de la Liberté", "Tunis", 85, 900, etage=3, ascenseur=True), prop)
    maison = agence.ajouter_bien(Maison("5 avenue des Jasmins", "La Marsa", 150, 1800, surface_jardin=200, garage=True), prop)
    agence.ajouter_bien(LocalCommercial("8 rue de Marseille", "Tunis", 40, 1200, activite="Boutique"), prop)

    c1 = agence.creer_contrat(appart, loc1, date(2026, 1, 1), date(2026, 12, 31), depot_garantie=1800)
    agence.creer_contrat(maison, loc2, date(2026, 6, 1), date(2027, 5, 31), depot_garantie=3600)

    # Paiements
    for loyer in c1.loyers[:10]:
        loyer.enregistrer_paiement(loyer.montant, loyer.date_echeance, ModePaiement.VIREMENT)
    print(c1.loyers[0].generer_quittance().texte(), "\n")

    # Maintenance
    demande = agence.signaler_panne(appart, "Fuite sous l'évier", Priorite.HAUTE)
    demande.assigner(tech)
    demande.demarrer()
    demande.terminer(cout=120)

    aujourdhui = date(2026, 11, 13)
    print("Biens :")
    for b in agence.biens:
        print(" -", b)
    print("\nTableau de bord au", aujourdhui)
    for cle, valeur in agence.tableau_de_bord(aujourdhui).items():
        print(f" {cle}: {valeur}")


if __name__ == "__main__":
    main()
