-- Function to update a product
CREATE OR REPLACE FUNCTION shopping.product_update(
    p_product_id UUID,
    p_sku VARCHAR,
    p_name VARCHAR,
    p_description TEXT,
    p_price DECIMAL,
    p_currency shopping.currency,
    p_product_category_id UUID,
    p_tag_ids INT[],
    p_last_mutation_instant TIMESTAMPTZ
)
RETURNS VOID
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE shopping.product
    SET
        sku = p_sku,
        name = p_name,
        description = p_description,
        price = p_price,
        currency = p_currency,
        product_category_id = p_product_category_id,
        tag_ids = p_tag_ids,
        last_mutation_instant = p_last_mutation_instant
    WHERE product_id = p_product_id;
END;
$$;

