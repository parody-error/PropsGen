namespace PropsGen.Services
{
    internal class Queries
    {
        public static readonly string DATABASE_NAMES =
@"select
  name 
from
  sys.databases
where
  name not in ('master', 'tempdb', 'model', 'msdb') order by name;";

        public static readonly string LAUNCHED_ENTITY_ID =
@"select
  ENTITY_ID
from
  ENTITY_LOCK_INFO
where
  LOCKED_BY is not null;";

        public static readonly string LAUNCHED_ENTITY_NAME = 
@"select
  COALESCE( W.WELL_NAME, W.DLS, UCG.CUSTOM_GROUP_NAME, E.ENTITY_NAME ) as ENTITY_NAME
from ENTITY E
  left join WELL W on (W.WELL_ID = E.FACILITY_ID)
  left join USER_CUSTOM_GROUPS UCG on (UCG.ENTITY_ID = E.ENTITY_ID)
where
  E.ENTITY_ID = @entityId;";
    }
}
