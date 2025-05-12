drop procedure if exists pro_cst_get_by_filter;
create procedure pro_cst_get_by_filter(in p_search_term varchar(500))
begin
    declare v_search_term varchar(504);
    set v_search_term = concat('.*', lower(trim(coalesce(p_search_term, ''))), '.*');

    select
        c.customer_id, c.name, c.email, c.created_at, c.updated_at
    from customer c
    where
        lower(c.name) regexp v_search_term or
        lower(c.email) regexp v_search_term
    ;
end
