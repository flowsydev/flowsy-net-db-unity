-- Function to create a new product category
CREATE OR REPLACE FUNCTION shopping.product_category_create(
    p_product_category_id UUID,
    p_code VARCHAR,
    p_name VARCHAR,
    p_description TEXT,
    p_creation_instant TIMESTAMPTZ
)
RETURNS VOID
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO shopping.product_category (
        product_category_id,
        code,
        name,
        description,
        creation_instant
    )
    VALUES (
        p_product_category_id,
        p_code,
        p_name,
        p_description,
        p_creation_instant
    );
END;
$$;

