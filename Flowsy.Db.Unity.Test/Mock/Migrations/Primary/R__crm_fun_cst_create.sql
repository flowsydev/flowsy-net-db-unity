set search_path to crm;

drop function if exists fun_cst_create;
create function fun_cst_create
(
    p_name varchar,
    p_email varchar,
    p_created_at timestamptz
) returns void as 
    $$
    begin
        set search_path to crm;
    
        insert into crm.customer
        (
            name,
            email,
            created_at
        )
        values
        (
            p_name,
            p_email,
            p_created_at
        );
    end;
    $$ language plpgsql;
    
