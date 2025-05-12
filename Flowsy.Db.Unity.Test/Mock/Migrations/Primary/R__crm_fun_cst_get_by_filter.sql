set search_path to crm;

drop function if exists fun_cst_get_by_filter;
create function fun_cst_get_by_filter
(
    p_search_term varchar
) returns table
(
    customer_id int,
    name varchar,
    email varchar,
    created_at timestamptz,
    updated_at timestamptz
) as 
    $$
    declare
        v_search_term varchar := '.*' || coalesce(ltrim(rtrim(p_search_term)), '') || '.*';
    begin
        set search_path to crm;
    
        return query
            select c.customer_id, c.name, c.email, c.created_at, c.updated_at
            from customer c
            where
                c.name ~* v_search_term or
                c.email ~* v_search_term
            ;
    end;
    $$ language plpgsql;
    
