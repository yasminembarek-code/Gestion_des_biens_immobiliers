"""Classes des biens immobiliers : Bien (abstraite), Appartement, Maison, LocalCommercial."""
from abc import ABC, abstractmethod
from itertools import count

from enums import StatutBien


class Bien(ABC):
    _compteur = count(1)

    def __init__(self, adresse, ville, surface, loyer_base):
        if surface <= 0 or loyer_base < 0:
            raise ValueError("Surface et loyer doivent être positifs.")
        self.id = next(Bien._compteur)
        self.adresse = adresse
        self.ville = ville
        self.surface = surface
        self.loyer_base = loyer_base
        self.statut = StatutBien.DISPONIBLE
        self.proprietaire = None
        self.contrats = []
        self.demandes_maintenance = []

    @abstractmethod
    def type_bien(self):
        """Retourne le type du bien."""

    def est_disponible(self):
        return self.statut == StatutBien.DISPONIBLE

    def changer_statut(self, statut):
        self.statut = statut

    def ajouter_demande_maintenance(self, demande):
        self.demandes_maintenance.append(demande)

    def cout_maintenance_total(self):
        return sum(d.cout for d in self.demandes_maintenance)

    def loyer_au_m2(self):
        return round(self.loyer_base / self.surface, 2)

    def __str__(self):
        return (f"{self.type_bien()} #{self.id} - {self.adresse}, {self.ville} "
                f"({self.surface} m², {self.loyer_base} /mois) [{self.statut.value}]")


class Appartement(Bien):
    def __init__(self, adresse, ville, surface, loyer_base, etage=0, ascenseur=False):
        super().__init__(adresse, ville, surface, loyer_base)
        self.etage = etage
        self.ascenseur = ascenseur

    def type_bien(self):
        return "Appartement"


class Maison(Bien):
    def __init__(self, adresse, ville, surface, loyer_base, surface_jardin=0, garage=False):
        super().__init__(adresse, ville, surface, loyer_base)
        self.surface_jardin = surface_jardin
        self.garage = garage

    def type_bien(self):
        return "Maison"


class LocalCommercial(Bien):
    def __init__(self, adresse, ville, surface, loyer_base, activite=""):
        super().__init__(adresse, ville, surface, loyer_base)
        self.activite = activite

    def type_bien(self):
        return "Local commercial"
