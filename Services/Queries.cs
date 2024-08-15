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

        public static readonly string GAS_PROPS =
@"select
  0 as GAS_PVT_CORR_ID,
  0 as GAS_VISC_CORR_ID,
  case AGP.GAS_TYPE_ID
    when 3 then 2
	else AGP.GAS_TYPE_ID
  end as GAS_TYPE_ID,
  case AGP.GAS_LIQUID_CORR_ID
    when 0 then 0
    when 6 then 1
	else 2
  end as GAS_LIQUID_CORR_ID,
  AGP.GAS_GRAVITY,
  AGP.CO2_FRACTION,
  AGP.N2_FRACTION,
  AGP.H2S_FRACTION,
  AGP.SEPARATOR_PRESSURE,
  AGP.SEPARATOR_TEMPERATURE,
  AGP.CONDENSATE_GAS_RATIO,
  1.0 as RV_OVER_RV_SAT
from ANALYSIS_GAS_PROP AGP
  join ANALYSIS_PROP AP on (AP.ANALYSIS_PROP_ID = AGP.ANALYSIS_PROP_ID)
  join ENTITY E on (E.FACILITY_ID = AP.FACILITY_ID)
where
  E.ENTITY_ID = @entityId;";

    }
}
