-- Function to create a new product
CREATE OR REPLACE FUNCTION shopping.product_create(
    p_product_id UUID,
    p_sku VARCHAR,
    p_name VARCHAR,
    p_description TEXT,
    p_price DECIMAL,
    p_currency shopping.currency,
    p_product_category_id UUID,
    p_tag_ids INT[],
    p_creation_instant TIMESTAMPTZ
)
RETURNS VOID
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO shopping.product (
        product_id,
        sku,
        name,
        description,
        price,
        currency,
        product_category_id,
        tag_ids,
        creation_instant
    )
    VALUES (
        p_product_id,
        p_sku,
        p_name,
        p_description,
        p_price,
        p_currency,
        p_product_category_id,
        p_tag_ids,
        p_creation_instant
    );
END;
$$;

