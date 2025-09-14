-- Function to update a product category
CREATE OR REPLACE FUNCTION shopping.product_category_update(
    p_product_category_id UUID,
    p_code VARCHAR(50),
    p_name VARCHAR(255),
    p_description TEXT,
    p_last_mutation_instant TIMESTAMPTZ
)
RETURNS VOID
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE shopping.product_category
    SET 
        code = p_code,
        name = p_name,
        description = p_description,
        last_mutation_instant = p_last_mutation_instant
    WHERE product_category_id = p_product_category_id;
END;
$$;

