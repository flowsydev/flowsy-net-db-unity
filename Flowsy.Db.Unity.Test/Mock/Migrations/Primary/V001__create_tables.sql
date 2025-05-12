create schema if not exists kernel;
create type kernel.currency as enum ('MXN', 'USD', 'EUR', 'GBP');

create schema if not exists crm;
create table crm.customer
(
    customer_id serial primary key,
    name varchar(255) not null unique,
    email varchar(255) not null unique,
    created_at timestamptz not null default clock_timestamp(),
    updated_at timestamptz null
);

create schema if not exists inventory;
create schema if not exists quoting;