SELECT* FROM OMAZON.PRODUCTO

SELECT* FROM OMAZON.CARRITO_COMPRAS

SELECT* FROM OMAZON.CATEGORIA

SELECT* FROM OMAZON.PRODUCTO

SELECT* FROM OMAZON.PRODUCTO_CATEGORIA

SELECT* FROM OMAZON.CATEGORIA
SELECT NOMBRE_PRODUCTO,NOMBRE_CATEGORIA, NOMBRE_PROVEEDOR
FROM OMAZON.PRODUCTO_CATEGORIA PC
JOIN OMAZON.CATEGORIA C
ON PC.ID_CATEGORIA=C.ID_CATEGORIA
JOIN OMAZON.PRODUCTO P
ON P.ID_PRODUCTO=PC.ID_PRODUCTO
Join OMAZON.PROVEEDOR PR
on PR.ID_PROVEEDOR=P.ID_PROVEEDOR

DELETE FROM OMAZON.CATEGORIA

SELECT* FROM OMAZON.CLAVE_ACCESO

SELECT* FROM OMAZON.SOLICITUD_PROVEEDOR