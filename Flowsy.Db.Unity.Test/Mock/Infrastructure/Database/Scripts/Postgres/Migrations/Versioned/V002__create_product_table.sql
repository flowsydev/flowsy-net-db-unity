CREATE TYPE shopping.currency AS ENUM ('Mxn', 'Usd', 'Eur');

-- Create the product table
CREATE TABLE shopping.product (
    product_id UUID PRIMARY KEY NOT NULL,
    sku VARCHAR(100) NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    price DECIMAL(18, 2) NOT NULL,
    currency shopping.currency NOT NULL,
    product_category_id UUID NOT NULL,
    creation_instant TIMESTAMPTZ NOT NULL,
    last_mutation_instant TIMESTAMPTZ,
    
    CONSTRAINT uk_product_sku UNIQUE (sku),
    CONSTRAINT fk_product_category FOREIGN KEY (product_category_id) REFERENCES shopping.product_category(product_category_id) ON DELETE RESTRICT
);

-- Create indexes for better performance
CREATE INDEX idx_product_sku ON shopping.product (sku);
CREATE INDEX idx_product_name ON shopping.product (name);
CREATE INDEX idx_product_price ON shopping.product (price);
CREATE INDEX idx_product_currency ON shopping.product (currency);
CREATE INDEX idx_product_category_id ON shopping.product (product_category_id);
CREATE INDEX idx_product_creation_instant ON shopping.product (creation_instant);
