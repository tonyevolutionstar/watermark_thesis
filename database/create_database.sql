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
	validacao INT -- 0 reject, 1 acept 
);

-- SELECT * FROM WATERMARK_QRCODE

--DROP TABLE forense_analises
CREATE TABLE forense_analises(
	id_doc INT FOREIGN KEY REFERENCES document(id_document),
	line1 varchar(200),
	line2 varchar(200),
	inter_point varchar(10), -- intersection point
	inter_char varchar(2),    -- character on that position
	line1_points varchar(200),
	line2_points varchar(200)
);

-- ex 1; qrcode1_l:qrcode4_r; qrcode2_l:qrcode4_l; 525, 1158; s; 354,37:574,1479;1125,37:354,1479 ';' columns
-- Select * from forense_analises