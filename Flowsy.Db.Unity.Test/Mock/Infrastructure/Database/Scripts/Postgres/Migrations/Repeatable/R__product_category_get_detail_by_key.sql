-- Function to read a product category by code
CREATE OR REPLACE FUNCTION shopping.product_category_get_detail_by_key
(
    p_key VARCHAR(50)
)
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
    WHERE pc.product_category_id::varchar = p_key
       OR pc.code = p_key
       OR pc.name = p_key
    LIMIT 1;
END;
$$;

