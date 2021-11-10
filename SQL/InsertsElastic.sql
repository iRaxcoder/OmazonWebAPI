INSERT INTO OMAZON.PRODUCTO (NOMBRE_PRODUCTO,STOCK,PRECIO,ID_PROVEEDOR,RUTA_IMAGEN)
VALUES
('Camiseta Nike',28,25000,4,'camisetaNike.jpg'),
('Camiseta Supreme', 15,23000,4,'CamisetaSuprem.jpg'),
('Halo 1 Xbox',2, 12000,4,'halo1.jpg'),
('Halo 2 Xbox', 5,11000,4,'halo2.jpg'),
('Lentes con luz azul',45,5000,4,'lentesLuzAzul.jpg'),
('Nike Revolution',120, 85000,4,'NikeRevo.jpg'),
('Sony PS5',50,650000,4, 'ps5p.jpg'),
('Xbox Series X',100, 520000,4,'XboxSeriesX.jpg')

SELECT* FROM OMAZON.PRODUCTO
SELECT* FROM OMAZON.CATEGORIA
select* from OMAZON.PROVEEDOR

INSERT INTO OMAZON.CATEGORIA(NOMBRE_CATEGORIA)
VALUES
('Camisetas'),
('Videojuegos'),
('Accesorios')

INSERT INTO OMAZON.PRODUCTO_CATEGORIA (ID_PRODUCTO,ID_CATEGORIA)
VALUES
(28,9),
(29,9),
(30,10),
(31,10),
(32,11),
(33,2),
(34,7),
(35,7)
