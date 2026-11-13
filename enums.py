"""Énumérations du domaine (statuts, priorités, modes de paiement)."""
from enum import Enum


class StatutBien(Enum):
    DISPONIBLE = "Disponible"
    LOUE = "Loué"
    EN_TRAVAUX = "En travaux"


class StatutContrat(Enum):
    ACTIF = "Actif"
    TERMINE = "Terminé"
    RESILIE = "Résilié"


class StatutLoyer(Enum):
    EN_ATTENTE = "En attente"
    PARTIEL = "Partiellement payé"
    PAYE = "Payé"
    EN_RETARD = "En retard"


class ModePaiement(Enum):
    ESPECES = "Espèces"
    VIREMENT = "Virement"
    CHEQUE = "Chèque"
    CARTE = "Carte bancaire"


class Priorite(Enum):
    BASSE = 1
    NORMALE = 2
    HAUTE = 3
    URGENTE = 4


class StatutMaintenance(Enum):
    OUVERTE = "Ouverte"
    EN_COURS = "En cours"
    TERMINEE = "Terminée"
    ANNULEE = "Annulée"
