---CREATE DATABASE AND PARAMETERS 

ALTER DATABASE Watermark SET OFFLINE;
-- se der erro ir à diretoria C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA e apagar o ficheiro Watermark.mdf e Watermark_log.ldf

DROP DATABASE Watermark;

CREATE DATABASE Watermark;

USE Watermark;

-- DROP TABLE document
CREATE TABLE document (
    id_document INT NOT NULL PRIMARY KEY,
	nome_ficheiro VARCHAR(100),
    n_registo VARCHAR(500),
	n_exemplar int,
    n_copy int,
    class_seg VARCHAR(500),
	estado_ex VARCHAR(500),
	formato_ex VARCHAR(500),
	utilizador VARCHAR(500),
	data_op VARCHAR(50),
	sigla_principal VARCHAR(500),
	posto_atual VARCHAR(500),
	dominio VARCHAR(500)
);

-- Select * from document;


-- DROP TABLE QRCODE
-- guarda os caracterers onde os qrcodes estão posicionadaos
CREATE TABLE QRCODE(
	id_qrcode INT IDENTITY(1,1) PRIMARY KEY,
	qrcode1 VARCHAR(12), -- CIMA, BAIXO, ESQUERDA,DIREITA
	qrcode2 VARCHAR(12),
	qrcode3 VARCHAR(12),
	qrcode4 VARCHAR(12),
	qrcode5 VARCHAR(12),
	qrcode6 VARCHAR(12),
	qrcode7 VARCHAR(12),
	qrcode8 VARCHAR(12),
	qrcode9 VARCHAR(12),
	date_time varchar(20)
);
-- Select * from QRCODE

-- DROP TABLE barcode
CREATE TABLE barcode(
	id_barcode INT IDENTITY(1,1) PRIMARY KEY,
	posicoes_qrcode VARCHAR(200),
	date_time VARCHAR(20) NOT NULL
);

-- SELECT * FROM barcode

-- DROP TABLE watermark_qrcode
CREATE TABLE watermark_qrcode(
	id_doc INT FOREIGN KEY REFERENCES document(id_document),
	id_barcode INT FOREIGN KEY REFERENCES barcode(id_barcode),
	id_qrcode INT FOREIGN KEY REFERENCES qrcode(id_qrcode),
	validacao INT -- 0 reject, 1 acept 
);

CREATE TABLE watermark_qrcode(
	id_doc INT FOREIGN KEY REFERENCES document(id_document),
	id_barcode INT FOREIGN KEY REFERENCES barcode(id_barcode),
	validacao INT -- 0 reject, 1 acept 
);

-- SELECT * FROM WATERMARK_QRCODE
