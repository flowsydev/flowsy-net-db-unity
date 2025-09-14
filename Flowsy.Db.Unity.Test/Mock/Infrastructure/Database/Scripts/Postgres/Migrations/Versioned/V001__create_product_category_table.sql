-- Create shopping schema if it doesn't exist
CREATE SCHEMA IF NOT EXISTS shopping;

-- Create the product_category table
CREATE TABLE shopping.product_category (
    product_category_id UUID PRIMARY KEY NOT NULL,
    code VARCHAR(50) NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    creation_instant TIMESTAMPTZ NOT NULL,
    last_mutation_instant TIMESTAMPTZ NULL,
    
    CONSTRAINT uk_product_category_code UNIQUE (code)
);

-- Create indexes for better performance
CREATE INDEX idx_product_category_code ON shopping.product_category (code);
CREATE INDEX idx_product_category_name ON shopping.product_category (name);
CREATE INDEX idx_product_category_creation_instant ON shopping.product_category (creation_instant);
