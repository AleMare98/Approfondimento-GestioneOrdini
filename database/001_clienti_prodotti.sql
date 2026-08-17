CREATE DATABASE IF NOT EXISTS GestioneOrdini
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;

USE GestioneOrdini;

CREATE TABLE IF NOT EXISTS Clienti (
    IdCliente INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(100) NOT NULL,
    Email VARCHAR(254) NOT NULL,
    CONSTRAINT PK_Clienti PRIMARY KEY (IdCliente),
    CONSTRAINT UQ_Clienti_Email UNIQUE (Email)
);

CREATE TABLE IF NOT EXISTS Prodotti (
    IdProdotto INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(150) NOT NULL,
    Prezzo DECIMAL(10, 2) NOT NULL,
    CONSTRAINT PK_Prodotti PRIMARY KEY (IdProdotto),
    CONSTRAINT CK_Prodotti_Prezzo_Positivo CHECK (Prezzo > 0)
);
