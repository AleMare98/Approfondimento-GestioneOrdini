USE GestioneOrdini;

CREATE TABLE IF NOT EXISTS Ordini (
    IdOrdine INT NOT NULL AUTO_INCREMENT,
    IdCliente INT NOT NULL,
    DataOrdine DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT PK_Ordini PRIMARY KEY (IdOrdine),
    CONSTRAINT FK_Ordini_Clienti
        FOREIGN KEY (IdCliente) REFERENCES Clienti (IdCliente)
);

CREATE TABLE IF NOT EXISTS RigheOrdine (
    IdOrdine INT NOT NULL,
    IdProdotto INT NOT NULL,
    Quantita INT NOT NULL,
    PrezzoUnitario DECIMAL(10, 2) NOT NULL,
    CONSTRAINT PK_RigheOrdine PRIMARY KEY (IdOrdine, IdProdotto),
    CONSTRAINT FK_RigheOrdine_Ordini
        FOREIGN KEY (IdOrdine) REFERENCES Ordini (IdOrdine),
    CONSTRAINT FK_RigheOrdine_Prodotti
        FOREIGN KEY (IdProdotto) REFERENCES Prodotti (IdProdotto),
    CONSTRAINT CK_RigheOrdine_Quantita_Positiva CHECK (Quantita > 0),
    CONSTRAINT CK_RigheOrdine_Prezzo_Positivo CHECK (PrezzoUnitario > 0)
);

CREATE INDEX IX_Ordini_IdCliente ON Ordini (IdCliente);
CREATE INDEX IX_RigheOrdine_IdProdotto ON RigheOrdine (IdProdotto);
