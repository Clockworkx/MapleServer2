﻿using Maple2Storage.Enums;

namespace MapleServer2.Database.Types;

public class BeautyShopItem
{
    public long Uid;
    public int ItemId;
    public Gender Gender;
    public ShopItemLabel Label;
    public short RequiredLevel;
    public int RequiredAchievementId;
    public byte RequiredAchievementGrade;
    public ShopCurrencyType CurrencyType;
    public int CurrencyCost;
    public int RequiredItemId;

    public BeautyShopItem(dynamic data)
    {
        Uid = data.uid;
        ItemId = data.item_id;
        Gender = (Gender) data.gender;
        Label = (ShopItemLabel) data.flag;
        RequiredLevel = data.required_level;
        RequiredAchievementId = data.required_achievement_id;
        RequiredAchievementGrade = (byte) data.required_achievement_grade;
        RequiredItemId = data.required_item_id;
        CurrencyType = (ShopCurrencyType) data.currency_type;
        CurrencyCost = data.currency_cost;
    }
}
