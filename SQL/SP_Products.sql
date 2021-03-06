ALTER PROCEDURE OMAZON.sp_BUSCAR_PRODUCTO_NOMBRE
@NOMBRE_BUSCAR VARCHAR(32)
AS
BEGIN
	IF EXISTS(SELECT* 
					FROM OMAZON.PRODUCTO P 
					WHERE P.NOMBRE_PRODUCTO LIKE '%'+@NOMBRE_BUSCAR+'%' )
	BEGIN
		SELECT
			P.ID_PRODUCTO,
			P.NOMBRE_PRODUCTO,
			P.PRECIO,
			P.STOCK,
			P.RUTA_IMAGEN,
			C.NOMBRE_CATEGORIA,
			PRO.NOMBRE_PROVEEDOR
				FROM OMAZON.PRODUCTO P
				   LEFT JOIN OMAZON.PRODUCTO_CATEGORIA PA
					  ON PA.ID_PRODUCTO=P.ID_PRODUCTO
					   LEFT JOIN OMAZON.CATEGORIA C
						 ON C.ID_CATEGORIA=PA.ID_CATEGORIA
							LEFT JOIN OMAZON.PROVEEDOR_PRODUCTO PP
								ON P.ID_PRODUCTO = PP.ID_PRODUCTO
									LEFT JOIN OMAZON.PROVEEDOR PRO
										ON PP.ID_PROVEEDOR = PRO.ID_PROVEEDOR
						   WHERE P.NOMBRE_PRODUCTO LIKE '%'+@NOMBRE_BUSCAR+'%'
	END
	ELSE
		BEGIN
		SELECT 'No se han encontrado productos asociados a "'+@NOMBRE_BUSCAR+'"'
		END
END
GO

ALTER PROCEDURE [PRODUCTO].[sp_SELECT_PRODUCTOS]
AS
BEGIN
	SELECT
	P.ID_PRODUCTO,
	P.NOMBRE_PRODUCTO,
	P.PRECIO,
	P.STOCK,
	P.RUTA_IMAGEN,
	C.NOMBRE_CATEGORIA,
	PRO.NOMBRE_PROVEEDOR
		FROM OMAZON.PRODUCTO P
			LEFT JOIN OMAZON.PRODUCTO_CATEGORIA PA
				ON PA.ID_PRODUCTO=P.ID_PRODUCTO
				LEFT JOIN OMAZON.CATEGORIA C
					ON C.ID_CATEGORIA=PA.ID_CATEGORIA
					LEFT JOIN OMAZON.PROVEEDOR_PRODUCTO PP
						ON P.ID_PRODUCTO = PP.ID_PRODUCTO
							LEFT JOIN OMAZON.PROVEEDOR PRO
								ON PP.ID_PROVEEDOR = PRO.ID_PROVEEDOR
END
GO

ALTER PROCEDURE OMAZON.sp_INSERTAR_PRODUCTO_PROVEEDOR_EXTERNO
@param_NOMBRE VARCHAR(50),
@param_STOCK INT,
@param_PRECIO VARCHAR(30),
@param_ID_PROVEEDOR INT,
@param_RUTA_IMAGEN VARCHAR(100),
@param_CATEGORIA VARCHAR(50)
AS
BEGIN
	DECLARE @local_PRODUCT_ID INT;
	DECLARE @local_CATEGORY_ID INT;

	

	INSERT INTO OMAZON.PRODUCTO (NOMBRE_PRODUCTO,STOCK,PRECIO,ID_PROVEEDOR,RUTA_IMAGEN)
	VALUES
	(
	@param_NOMBRE,
	@param_STOCK,
	@param_PRECIO,
	@param_ID_PROVEEDOR,
	@param_RUTA_IMAGEN
	)

	SET @local_PRODUCT_ID=SCOPE_IDENTITY();

	-----CATEGORY

	IF EXISTS (SELECT* FROM OMAZON.CATEGORIA WHERE NOMBRE_CATEGORIA=@param_CATEGORIA)
		BEGIN
			SET @local_CATEGORY_ID= (SELECT ID_CATEGORIA FROM OMAZON.CATEGORIA WHERE NOMBRE_CATEGORIA=@param_CATEGORIA)
		END
	ELSE
		BEGIN
		INSERT INTO OMAZON.CATEGORIA (NOMBRE_CATEGORIA) VALUES (@param_CATEGORIA)
		SET @local_CATEGORY_ID= SCOPE_IDENTITY();
		END

	-----INSERT RELATIONSHIP
	INSERT INTO OMAZON.PRODUCTO_CATEGORIA (ID_PRODUCTO,ID_CATEGORIA)
	VALUES
	(@local_PRODUCT_ID,@local_CATEGORY_ID)
	
END