-- Function to list all product categories
CREATE OR REPLACE FUNCTION shopping.product_category_get_all()
RETURNS TABLE(
    product_category_id UUID,
    code VARCHAR,
    name VARCHAR,
    description TEXT,
    creation_instant TIMESTAMPTZ,
    last_mutation_instant TIMESTAMPTZ
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        pc.product_category_id,
        pc.code,
        pc.name,
        pc.description,
        pc.creation_instant,
        pc.last_mutation_instant
    FROM shopping.product_category pc
    ORDER BY pc.name ASC;
END;
$$;

