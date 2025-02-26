//////////////////////////////////
//Auto-generated. Do NOT modify!//
//////////////////////////////////

import { MessageKey, QueryKey, Type, EnumType, registerSymbol } from '../../Signum.React/Scripts/Reflection'
import * as Entities from '../../Signum.React/Scripts/Signum.Entities'
import * as Basics from '../../Signum.React/Scripts/Signum.Entities.Basics'
import * as Authorization from '../Authorization/Signum.Entities.Authorization'


export const HolidayCalendarEntity = new Type<HolidayCalendarEntity>("HolidayCalendar");
export interface HolidayCalendarEntity extends Entities.Entity {
  Type: "HolidayCalendar";
  name: string;
  holidays: Entities.MList<HolidayEmbedded>;
}

export module HolidayCalendarOperation {
  export const Save : Entities.ExecuteSymbol<HolidayCalendarEntity> = registerSymbol("Operation", "HolidayCalendarOperation.Save");
  export const Delete : Entities.DeleteSymbol<HolidayCalendarEntity> = registerSymbol("Operation", "HolidayCalendarOperation.Delete");
}

export const HolidayEmbedded = new Type<HolidayEmbedded>("HolidayEmbedded");
export interface HolidayEmbedded extends Entities.EmbeddedEntity {
  Type: "HolidayEmbedded";
  date: string;
  name: string | null;
}

export interface IScheduleRuleEntity extends Entities.Entity {
  startingOn: string;
}

export interface ITaskEntity extends Entities.Entity {
}

export module ITaskMessage {
  export const Execute = new MessageKey("ITaskMessage", "Execute");
  export const Executions = new MessageKey("ITaskMessage", "Executions");
  export const LastExecution = new MessageKey("ITaskMessage", "LastExecution");
  export const ExceptionLines = new MessageKey("ITaskMessage", "ExceptionLines");
}

export module ITaskOperation {
  export const ExecuteSync : Entities.ConstructSymbol_From<ScheduledTaskLogEntity, ITaskEntity> = registerSymbol("Operation", "ITaskOperation.ExecuteSync");
  export const ExecuteAsync : Entities.ExecuteSymbol<ITaskEntity> = registerSymbol("Operation", "ITaskOperation.ExecuteAsync");
}

export const ScheduledTaskEntity = new Type<ScheduledTaskEntity>("ScheduledTask");
export interface ScheduledTaskEntity extends Entities.Entity {
  Type: "ScheduledTask";
  rule: IScheduleRuleEntity;
  task: ITaskEntity;
  suspended: boolean;
  machineName: string;
  user: Entities.Lite<Basics.IUserEntity>;
  applicationName: string;
}

export const ScheduledTaskLogEntity = new Type<ScheduledTaskLogEntity>("ScheduledTaskLog");
export interface ScheduledTaskLogEntity extends Entities.Entity {
  Type: "ScheduledTaskLog";
  task: ITaskEntity;
  scheduledTask: ScheduledTaskEntity | null;
  user: Entities.Lite<Basics.IUserEntity>;
  startTime: string;
  endTime: string | null;
  machineName: string;
  applicationName: string;
  productEntity: Entities.Lite<Entities.Entity> | null;
  exception: Entities.Lite<Basics.ExceptionEntity> | null;
  remarks: string | null;
}

export module ScheduledTaskLogOperation {
  export const CancelRunningTask : Entities.ExecuteSymbol<ScheduledTaskLogEntity> = registerSymbol("Operation", "ScheduledTaskLogOperation.CancelRunningTask");
}

export module ScheduledTaskOperation {
  export const Save : Entities.ExecuteSymbol<ScheduledTaskEntity> = registerSymbol("Operation", "ScheduledTaskOperation.Save");
  export const Delete : Entities.DeleteSymbol<ScheduledTaskEntity> = registerSymbol("Operation", "ScheduledTaskOperation.Delete");
}

export module SchedulerMessage {
  export const Each0Minutes = new MessageKey("SchedulerMessage", "Each0Minutes");
  export const ScheduleRuleWeekDaysDN_AndHoliday = new MessageKey("SchedulerMessage", "ScheduleRuleWeekDaysDN_AndHoliday");
  export const ScheduleRuleWeekDaysDN_At = new MessageKey("SchedulerMessage", "ScheduleRuleWeekDaysDN_At");
  export const ScheduleRuleWeekDaysDN_ButHoliday = new MessageKey("SchedulerMessage", "ScheduleRuleWeekDaysDN_ButHoliday");
  export const ScheduleRuleWeekDaysDN_Mo = new MessageKey("SchedulerMessage", "ScheduleRuleWeekDaysDN_Mo");
  export const ScheduleRuleWeekDaysDN_Tu = new MessageKey("SchedulerMessage", "ScheduleRuleWeekDaysDN_Tu");
  export const ScheduleRuleWeekDaysDN_We = new MessageKey("SchedulerMessage", "ScheduleRuleWeekDaysDN_We");
  export const ScheduleRuleWeekDaysDN_Th = new MessageKey("SchedulerMessage", "ScheduleRuleWeekDaysDN_Th");
  export const ScheduleRuleWeekDaysDN_Fr = new MessageKey("SchedulerMessage", "ScheduleRuleWeekDaysDN_Fr");
  export const ScheduleRuleWeekDaysDN_Sa = new MessageKey("SchedulerMessage", "ScheduleRuleWeekDaysDN_Sa");
  export const ScheduleRuleWeekDaysDN_Su = new MessageKey("SchedulerMessage", "ScheduleRuleWeekDaysDN_Su");
  export const Day0At1In2 = new MessageKey("SchedulerMessage", "Day0At1In2");
  export const TaskIsNotRunning = new MessageKey("SchedulerMessage", "TaskIsNotRunning");
}

export module SchedulerPermission {
  export const ViewSchedulerPanel : Authorization.PermissionSymbol = registerSymbol("Permission", "SchedulerPermission.ViewSchedulerPanel");
}

export const SchedulerTaskExceptionLineEntity = new Type<SchedulerTaskExceptionLineEntity>("SchedulerTaskExceptionLine");
export interface SchedulerTaskExceptionLineEntity extends Entities.Entity {
  Type: "SchedulerTaskExceptionLine";
  elementInfo: string | null;
  schedulerTaskLog: Entities.Lite<ScheduledTaskLogEntity> | null;
  exception: Entities.Lite<Basics.ExceptionEntity>;
}

export const ScheduleRuleMinutelyEntity = new Type<ScheduleRuleMinutelyEntity>("ScheduleRuleMinutely");
export interface ScheduleRuleMinutelyEntity extends Entities.Entity, IScheduleRuleEntity {
  Type: "ScheduleRuleMinutely";
  startingOn: string;
  eachMinutes: number;
  isAligned: boolean;
}

export const ScheduleRuleMonthsEntity = new Type<ScheduleRuleMonthsEntity>("ScheduleRuleMonths");
export interface ScheduleRuleMonthsEntity extends Entities.Entity, IScheduleRuleEntity {
  Type: "ScheduleRuleMonths";
  startingOn: string;
  january: boolean;
  february: boolean;
  march: boolean;
  april: boolean;
  may: boolean;
  june: boolean;
  july: boolean;
  august: boolean;
  september: boolean;
  october: boolean;
  november: boolean;
  december: boolean;
}

export const ScheduleRuleWeekDaysEntity = new Type<ScheduleRuleWeekDaysEntity>("ScheduleRuleWeekDays");
export interface ScheduleRuleWeekDaysEntity extends Entities.Entity, IScheduleRuleEntity {
  Type: "ScheduleRuleWeekDays";
  startingOn: string;
  monday: boolean;
  tuesday: boolean;
  wednesday: boolean;
  thursday: boolean;
  friday: boolean;
  saturday: boolean;
  sunday: boolean;
  calendar: HolidayCalendarEntity | null;
  holiday: boolean;
}

export const SimpleTaskSymbol = new Type<SimpleTaskSymbol>("SimpleTask");
export interface SimpleTaskSymbol extends Entities.Symbol, ITaskEntity {
  Type: "SimpleTask";
}

export const SystemEventLogEntity = new Type<SystemEventLogEntity>("SystemEventLog");
export interface SystemEventLogEntity extends Entities.Entity {
  Type: "SystemEventLog";
  machineName: string;
  date: string;
  user: Entities.Lite<Basics.IUserEntity> | null;
  eventType: string;
  exception: Entities.Lite<Basics.ExceptionEntity> | null;
}


