-- Function to list all product categories
CREATE OR REPLACE FUNCTION shopping.product_category_get_overview_by_key
(
    p_key varchar
)
RETURNS TABLE(
    product_category_id UUID,
    code VARCHAR,
    name VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        pc.product_category_id,
        pc.code,
        pc.name
    FROM shopping.product_category pc
    WHERE pc.product_category_id::varchar = p_key
       OR pc.code = p_key
       OR pc.name = p_key
    LIMIT 1;
END;
$$;

