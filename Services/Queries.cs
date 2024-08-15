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

        public static readonly string RESERVOIR_PROPS =
@"select
  RD.RESERVOIR_TEMPERATURE,
  RD.INITIAL_RESERVOIR_PRESSURE,
  RD.NET_PAY,
  RD.TOTAL_POROSITY,
  RD.INITIAL_GAS_SATURATION,
  RD.INITIAL_OIL_SATURATION,
  RD.INITIAL_WATER_SATURATION,
  AFP.FORMATION_COMPRESSIBILITY
from RESERVOIR_DATA RD
  join ANALYSIS_PROP AP on (AP.ANALYSIS_PROP_ID = RD.ANALYSIS_PROP_ID) 
  join ANALYSIS_FORMATION_PROP AFP on (AFP.ANALYSIS_PROP_ID = RD.ANALYSIS_PROP_ID)
  join ENTITY E on (E.FACILITY_ID = AP.FACILITY_ID)
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

        public static readonly string OIL_PROPS =
@"select
  0 as OIL_PVT_CORR_ID,
  0 as OIL_VISC_CORR_ID,
  AOP.OIL_GRAVITY,
  AOP.BUBBLE_POINT_PRESSURE,
  AOP.INITIAL_SOLUTION_GOR
from ANALYSIS_OIL_PROP AOP
  join ANALYSIS_PROP AP on (AP.ANALYSIS_PROP_ID = AOP.ANALYSIS_PROP_ID)
  join ENTITY E on (E.FACILITY_ID = AP.FACILITY_ID)
where
  E.ENTITY_ID = @entityId;";

        public static readonly string WATER_PROPS =
@"select
  0 as GENERAL_CORR_ID,
  AWP.WATER_GRAVITY,
  AWP.SALINITY,
  AWP.IS_WTR_GAS_SATURATED
from ANALYSIS_WATER_PROP AWP
  join ANALYSIS_PROP AP on (AP.ANALYSIS_PROP_ID = AWP.ANALYSIS_PROP_ID)
  join ENTITY E on (E.FACILITY_ID = AP.FACILITY_ID)
where
  E.ENTITY_ID = @entityId";
    }
}
