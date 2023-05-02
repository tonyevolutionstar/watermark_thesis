---CREATE DATABASE AND PARAMETERS 
ALTER DATABASE Watermark SET OFFLINE;
-- se der erro ir à diretoria C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA e apagar o ficheiro Watermark.mdf e Watermark_log.ldf

DROP DATABASE IF EXISTS Watermark;

CREATE DATABASE Watermark;

USE Watermark;

DROP TABLE IF EXISTS forense_analises;
DROP TABLE IF EXISTS position_char_file;
DROP TABLE IF EXISTS watermark;
DROP TABLE IF EXISTS barcode;
DROP TABLE IF EXISTS dimensions_document;
DROP TABLE IF EXISTS document;

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
	dominio VARCHAR(500),
	date_time varchar(50)
);

CREATE TABLE barcode(
	id_barcode INT IDENTITY(1,1) PRIMARY KEY,
	positions_circleX VARCHAR(200),
	date_time VARCHAR(20) NOT NULL
);

CREATE TABLE watermark(
	id_doc INT FOREIGN KEY REFERENCES document(id_document),
	id_barcode INT FOREIGN KEY REFERENCES barcode(id_barcode),
	validacao INT, -- 0 reject, 1 accept
	x INT, -- start x position barcode
	y INT,  -- start y position barcode
	x2 INT, -- end x position barcode
	y2 INT, -- end y position barcode
	x_39 INT,
	y_39 INT,
	x2_39 INT,
	y2_39 INT
);

CREATE TABLE dimensions_document(
id_doc INT FOREIGN KEY REFERENCES document(id_document),
width int,
height int,
width_bmp int,
height_bmp int 
);

CREATE TABLE position_char_file(
	id_doc INT FOREIGN KEY REFERENCES document(id_document),
	value_char char(20),
	start_x int,
	start_y int,
	stop_x int,
	stop_y int
);

CREATE TABLE forense_analises(
	id_doc INT FOREIGN KEY REFERENCES document(id_document),
	line1 varchar(200),
	line2 varchar(200),
	inter_point varchar(100), -- intersection point
	inter_char nvarchar(2),    -- character on that position
	line1_points varchar(200),
	line2_points varchar(200)
);



-- Select * from document;
-- Select * from barcode;
-- Select * from watermark;
-- Select * from forense_analises;
-- Select * from position_char_file;
-- Select * from dimensions_document;