"""Classes des personnes : Personne (abstraite), Proprietaire, Locataire, Technicien."""
from abc import ABC, abstractmethod
from itertools import count


class Personne(ABC):
    _compteur = count(1)

    def __init__(self, nom, prenom, email, telephone):
        self.id = next(Personne._compteur)
        self.nom = nom
        self.prenom = prenom
        self.email = email
        self.telephone = telephone

    @property
    def nom_complet(self):
        return f"{self.prenom} {self.nom}"

    @abstractmethod
    def role(self):
        """Retourne le rôle de la personne dans le système."""

    def __str__(self):
        return f"{self.role()} #{self.id} - {self.nom_complet}"


class Proprietaire(Personne):
    def __init__(self, nom, prenom, email, telephone):
        super().__init__(nom, prenom, email, telephone)
        self.biens = []

    def role(self):
        return "Propriétaire"

    def ajouter_bien(self, bien):
        if bien not in self.biens:
            self.biens.append(bien)
            bien.proprietaire = self

    def revenus_potentiels(self):
        return sum(b.loyer_base for b in self.biens)


class Locataire(Personne):
    def __init__(self, nom, prenom, email, telephone, profession=""):
        super().__init__(nom, prenom, email, telephone)
        self.profession = profession
        self.contrats = []

    def role(self):
        return "Locataire"

    def contrats_actifs(self, aujourdhui=None):
        return [c for c in self.contrats if c.est_actif(aujourdhui)]


class Technicien(Personne):
    def __init__(self, nom, prenom, email, telephone, specialite):
        super().__init__(nom, prenom, email, telephone)
        self.specialite = specialite
        self.interventions = []

    def role(self):
        return f"Technicien ({self.specialite})"

    def charge_de_travail(self):
        return sum(1 for d in self.interventions if d.statut.name == "EN_COURS")
