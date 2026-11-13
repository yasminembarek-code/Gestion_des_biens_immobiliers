"""Classes financières : Loyer, Paiement, Quittance."""
from datetime import date
from itertools import count

from enums import StatutLoyer, ModePaiement


class Paiement:
    _compteur = count(1)

    def __init__(self, loyer, montant, date_paiement=None, mode=ModePaiement.VIREMENT):
        if montant <= 0:
            raise ValueError("Le montant doit être positif.")
        self.id = next(Paiement._compteur)
        self.loyer = loyer
        self.montant = montant
        self.date_paiement = date_paiement or date.today()
        self.mode = mode

    def __str__(self):
        return f"Paiement #{self.id}: {self.montant} le {self.date_paiement} ({self.mode.value})"


class Loyer:
    TAUX_PENALITE = 0.05

    _compteur = count(1)

    def __init__(self, contrat, mois, montant, date_echeance):
        self.id = next(Loyer._compteur)
        self.contrat = contrat
        self.mois = mois
        self.montant = montant
        self.date_echeance = date_echeance
        self.statut = StatutLoyer.EN_ATTENTE
        self.paiements = []

    @property
    def montant_paye(self):
        return sum(p.montant for p in self.paiements)

    @property
    def reste_a_payer(self):
        return max(0, round(self.montant - self.montant_paye, 2))

    def enregistrer_paiement(self, montant, date_paiement=None, mode=ModePaiement.VIREMENT):
        if montant > self.reste_a_payer:
            raise ValueError("Le paiement dépasse le reste à payer.")
        paiement = Paiement(self, montant, date_paiement, mode)
        self.paiements.append(paiement)
        self.mettre_a_jour_statut(paiement.date_paiement)
        return paiement

    def mettre_a_jour_statut(self, aujourdhui=None):
        aujourdhui = aujourdhui or date.today()
        if self.reste_a_payer == 0:
            self.statut = StatutLoyer.PAYE
        elif aujourdhui > self.date_echeance:
            self.statut = StatutLoyer.EN_RETARD
        elif self.montant_paye > 0:
            self.statut = StatutLoyer.PARTIEL
        else:
            self.statut = StatutLoyer.EN_ATTENTE
        return self.statut

    def penalite(self, aujourdhui=None):
        if self.mettre_a_jour_statut(aujourdhui) == StatutLoyer.EN_RETARD:
            return round(self.reste_a_payer * self.TAUX_PENALITE, 2)
        return 0.0

    def generer_quittance(self):
        if self.statut != StatutLoyer.PAYE:
            raise ValueError("Une quittance n'est émise que pour un loyer soldé.")
        return Quittance(self)

    def __str__(self):
        return (f"Loyer #{self.id} {self.mois:%m/%Y}: {self.montant} "
                f"[{self.statut.value}] reste {self.reste_a_payer}")


class Quittance:
    _compteur = count(1)

    def __init__(self, loyer, date_emission=None):
        self.id = next(Quittance._compteur)
        self.loyer = loyer
        self.date_emission = date_emission or date.today()

    def texte(self):
        c = self.loyer.contrat
        return (f"QUITTANCE DE LOYER n°{self.id}\n"
                f"Locataire : {c.locataire.nom_complet}\n"
                f"Bien : {c.bien.adresse}, {c.bien.ville}\n"
                f"Période : {self.loyer.mois:%m/%Y}\n"
                f"Montant réglé : {self.loyer.montant}\n"
                f"Émise le : {self.date_emission}")
