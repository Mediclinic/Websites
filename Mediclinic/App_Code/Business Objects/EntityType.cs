using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//
// this is unrelated to Entity table in the database
//
public class EntityType
{

    // enum of types
    public enum EntityTypeEnum { Patient, Staff, Referrer, Organisation, Site, None };


    // creation methods
    public static EntityType GetByType(EntityTypeEnum entityType)
    {
        return new EntityType(entityType);
    }
    public static EntityType GetByString(string entityType)
    {
        return new EntityType(StringToEntityType(entityType));
    }


    // variable(s) and get/set methods
    protected EntityTypeEnum _type;
    public EntityTypeEnum Type
    {
        get { return this._type; }
    }
    public string String
    {
        get { return EntityTypeToString(this._type); }
    }


    // constructor (hidden to force using creation methods
    protected EntityType(EntityTypeEnum _type)
    {
        this._type = _type;
    }


    // conversion: type -> string
    protected static string EntityTypeToString(EntityTypeEnum entityType)
    {
        if (entityType == EntityTypeEnum.Patient)
            return "patient";
        else if (entityType == EntityTypeEnum.Staff)
            return "staff";
        else if (entityType == EntityTypeEnum.Referrer)
            return "referrer";
        else if (entityType == EntityTypeEnum.Organisation)
            return "organisation";
        else if (entityType == EntityTypeEnum.Site)
            return "site";
        else
            return "";
    }

    // conversion: string -> type
    protected static EntityTypeEnum StringToEntityType(string entityType)
    {
        if (entityType != null && entityType.ToLower() == "patient")
            return EntityTypeEnum.Patient;
        else if (entityType != null && entityType.ToLower() == "staff")
            return EntityTypeEnum.Staff;
        else if (entityType != null && entityType.ToLower() == "referrer")
            return EntityTypeEnum.Referrer;
        else if (entityType != null && entityType.ToLower() == "organisation")
            return EntityTypeEnum.Organisation;
        else if (entityType != null && entityType.ToLower() == "site")
            return EntityTypeEnum.Site;
        else
            return EntityTypeEnum.None;
    }



}