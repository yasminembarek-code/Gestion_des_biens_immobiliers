"""Classe GestionImmobiliere : point d'entrée du système (façade + statistiques)."""
from datetime import date, timedelta

from enums import StatutBien, StatutMaintenance
from contrat import Contrat
from maintenance import DemandeMaintenance


class GestionImmobiliere:
    def __init__(self, nom_agence):
        self.nom_agence = nom_agence
        self.proprietaires = []
        self.locataires = []
        self.techniciens = []
        self.biens = []
        self.contrats = []
        self.demandes = []

    # --- Enregistrement ---
    def ajouter_proprietaire(self, proprietaire):
        self.proprietaires.append(proprietaire)
        return proprietaire

    def ajouter_locataire(self, locataire):
        self.locataires.append(locataire)
        return locataire

    def ajouter_technicien(self, technicien):
        self.techniciens.append(technicien)
        return technicien

    def ajouter_bien(self, bien, proprietaire):
        proprietaire.ajouter_bien(bien)
        self.biens.append(bien)
        return bien

    # --- Opérations ---
    def creer_contrat(self, bien, locataire, date_debut, date_fin, depot_garantie=0):
        contrat = Contrat(bien, locataire, date_debut, date_fin, depot_garantie)
        contrat.generer_echeancier()
        self.contrats.append(contrat)
        return contrat

    def signaler_panne(self, bien, description, priorite):
        demande = DemandeMaintenance(bien, description, priorite)
        self.demandes.append(demande)
        return demande

    def biens_disponibles(self):
        return [b for b in self.biens if b.est_disponible()]

    # --- Statistiques / tableau de bord ---
    def taux_occupation(self):
        if not self.biens:
            return 0.0
        loues = sum(1 for b in self.biens if b.statut == StatutBien.LOUE)
        return round(100 * loues / len(self.biens), 1)

    def loyers_impayes(self, aujourdhui=None):
        return [l for c in self.contrats for l in c.loyers_impayes(aujourdhui)]

    def contrats_a_echeance(self, jours=60, aujourdhui=None):
        aujourdhui = aujourdhui or date.today()
        limite = aujourdhui + timedelta(days=jours)
        return [c for c in self.contrats
                if c.est_actif(aujourdhui) and c.date_fin <= limite]

    def revenus_encaisses(self):
        return sum(l.montant_paye for c in self.contrats for l in c.loyers)

    def revenus_attendus(self, jusqua=None):
        jusqua = jusqua or date.today()
        return sum(l.montant for c in self.contrats for l in c.loyers
                   if l.date_echeance <= jusqua)

    def depenses_maintenance(self):
        return sum(d.cout for d in self.demandes if d.statut == StatutMaintenance.TERMINEE)

    def tableau_de_bord(self, aujourdhui=None):
        return {
            "taux_occupation_%": self.taux_occupation(),
            "loyers_impayes": len(self.loyers_impayes(aujourdhui)),
            "contrats_bientot_termines": len(self.contrats_a_echeance(60, aujourdhui)),
            "revenus_encaisses": self.revenus_encaisses(),
            "depenses_maintenance": self.depenses_maintenance(),
        }
