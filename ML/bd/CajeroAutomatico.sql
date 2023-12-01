CREATE DATABASE CajeroAutomatico

GO
USE CajeroAutomatico

GO

CREATE TABLE ATM(
IdATM INT PRIMARY KEY IDENTITY(1,1),
Saldo DECIMAL(18,2),
)

CREATE TABLE Denominacion(
IdDenominacion INT PRIMARY KEY IDENTITY(1,1),
Denominación INT,
)

CREATE TABLE ATMDenominacion(
IdATMDenominacion INT PRIMARY KEY IDENTITY(1,1),
IdATM INT REFERENCES ATM(IdATM),
IdDenominacion INT REFERENCES Denominacion (IdDenominacion),
Cantidad INT
)

CREATE TABLE Cuenta(
IdCuenta INT PRIMARY KEY IDENTITY(1,1),
Nombre VARCHAR(100),
NumeroCuenta INT,
Saldo Decimal(18,2),
)

CREATE TABLE Movimiento(
IdMovimiento INT PRIMARY KEY IDENTITY(1,1),
TipoMovimiento VARCHAR (50),
Monto INT,
FechaMovimiento DATE,
IdCuenta INT REFERENCES Cuenta (IdCuenta)
)

CREATE TABLE MovimientoDenominacion(
IdMovimientoDenominacion INT PRIMARY KEY IDENTITY(1,1),
IdMovimiento INT REFERENCES Movimiento(IdMovimiento),
IdDenominacion INT REFERENCES Denominacion (IdDenominacion),
Cantidad INT
)

GO

INSERT INTO [dbo].[ATM]
           ([Saldo])
     VALUES
           (500000)
GO

INSERT INTO [dbo].[Denominacion]
           ([Denominación])
     VALUES
           (1000),
		   (500),
		   (200),
		   (100),
		   (50),
		   (20)
GO

INSERT INTO [dbo].[ATMDenominacion]
           ([IdATM]
           ,[IdDenominacion]
           ,[Cantidad])
     VALUES
           (1, 1, 100),
           (1, 2, 200),
           (1, 3, 500),
           (1, 4, 1000),
           (1, 5, 1000),
           (1, 6, 2500)
GO
