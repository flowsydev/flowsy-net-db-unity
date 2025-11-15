-- Function to get products by tag ids
CREATE OR REPLACE FUNCTION shopping.product_get_by_tag_ids(
    p_tag_ids INT[]
)
RETURNS TABLE (
    product_id UUID,
    sku VARCHAR,
    name VARCHAR,
    description TEXT,
    price DECIMAL,
    currency shopping.currency,
    product_category_id UUID,
    tag_ids INT[],
    creation_instant TIMESTAMPTZ,
    last_mutation_instant TIMESTAMPTZ
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        p.product_id,
        p.sku,
        p.name,
        p.description,
        p.price,
        p.currency,
        p.product_category_id,
        p.tag_ids,
        p.creation_instant,
        p.last_mutation_instant
    FROM shopping.product p
    WHERE p.tag_ids && p_tag_ids  -- Overlap operator: checks if arrays have any common elements
    ORDER BY p.creation_instant DESC;
END;
$$;

