-- Function to delete a product category
CREATE OR REPLACE FUNCTION shopping.product_category_delete_by_id(
    p_product_category_id UUID
)
RETURNS VOID
LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM shopping.product_category
    WHERE product_category_id = p_product_category_id;
END;
$$;

