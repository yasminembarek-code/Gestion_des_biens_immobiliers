"""Classe Contrat : lie un bien à un locataire et génère l'échéancier des loyers."""
import calendar
from datetime import date
from itertools import count

from enums import StatutContrat, StatutBien
from finances import Loyer


class Contrat:
    JOUR_ECHEANCE = 5
    _compteur = count(1)

    def __init__(self, bien, locataire, date_debut, date_fin, depot_garantie=0, loyer_mensuel=None):
        if date_fin <= date_debut:
            raise ValueError("La date de fin doit suivre la date de début.")
        if not bien.est_disponible():
            raise ValueError("Ce bien n'est pas disponible à la location.")
        self.id = next(Contrat._compteur)
        self.bien = bien
        self.locataire = locataire
        self.date_debut = date_debut
        self.date_fin = date_fin
        self.depot_garantie = depot_garantie
        self.loyer_mensuel = loyer_mensuel if loyer_mensuel is not None else bien.loyer_base
        self.statut = StatutContrat.ACTIF
        self.loyers = []

        bien.contrats.append(self)
        bien.changer_statut(StatutBien.LOUE)
        locataire.contrats.append(self)

    def est_actif(self, aujourdhui=None):
        aujourdhui = aujourdhui or date.today()
        return (self.statut == StatutContrat.ACTIF
                and self.date_debut <= aujourdhui <= self.date_fin)

    def jours_restants(self, aujourdhui=None):
        aujourdhui = aujourdhui or date.today()
        return (self.date_fin - aujourdhui).days

    def generer_echeancier(self):
        """Crée un loyer par mois entre la date de début et la date de fin."""
        self.loyers = []
        annee, mois = self.date_debut.year, self.date_debut.month
        while date(annee, mois, 1) <= self.date_fin:
            jour = min(self.JOUR_ECHEANCE, calendar.monthrange(annee, mois)[1])
            self.loyers.append(
                Loyer(self, date(annee, mois, 1), self.loyer_mensuel, date(annee, mois, jour)))
            mois += 1
            if mois > 12:
                annee, mois = annee + 1, 1
        return self.loyers

    def reviser_loyer(self, taux):
        """Révision annuelle : applique un taux (ex. 0.02 = +2 %) aux loyers non payés."""
        self.loyer_mensuel = round(self.loyer_mensuel * (1 + taux), 2)
        for loyer in self.loyers:
            if loyer.montant_paye == 0:
                loyer.montant = self.loyer_mensuel
        return self.loyer_mensuel

    def loyers_impayes(self, aujourdhui=None):
        return [l for l in self.loyers
                if l.mettre_a_jour_statut(aujourdhui).name == "EN_RETARD"]

    def resilier(self):
        self.statut = StatutContrat.RESILIE
        self.bien.changer_statut(StatutBien.DISPONIBLE)

    def terminer(self):
        self.statut = StatutContrat.TERMINE
        self.bien.changer_statut(StatutBien.DISPONIBLE)

    def __str__(self):
        return (f"Contrat #{self.id}: {self.locataire.nom_complet} / {self.bien.adresse} "
                f"({self.date_debut} -> {self.date_fin}) [{self.statut.value}]")
