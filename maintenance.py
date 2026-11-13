"""Classe DemandeMaintenance : suivi des interventions sur un bien."""
from datetime import date
from itertools import count

from enums import Priorite, StatutMaintenance


class DemandeMaintenance:
    _compteur = count(1)

    def __init__(self, bien, description, priorite=Priorite.NORMALE, date_demande=None):
        self.id = next(DemandeMaintenance._compteur)
        self.bien = bien
        self.description = description
        self.priorite = priorite
        self.date_demande = date_demande or date.today()
        self.statut = StatutMaintenance.OUVERTE
        self.technicien = None
        self.cout = 0.0
        self.date_cloture = None
        bien.ajouter_demande_maintenance(self)

    def assigner(self, technicien):
        self.technicien = technicien
        technicien.interventions.append(self)

    def demarrer(self):
        if self.technicien is None:
            raise ValueError("Assignez un technicien avant de démarrer.")
        self.statut = StatutMaintenance.EN_COURS

    def terminer(self, cout, date_cloture=None):
        if cout < 0:
            raise ValueError("Le coût ne peut pas être négatif.")
        self.cout = cout
        self.date_cloture = date_cloture or date.today()
        self.statut = StatutMaintenance.TERMINEE

    def annuler(self):
        self.statut = StatutMaintenance.ANNULEE

    def __str__(self):
        return (f"Maintenance #{self.id} [{self.priorite.name}] {self.description} "
                f"- {self.statut.value} ({self.cout})")
