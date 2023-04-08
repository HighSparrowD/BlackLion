import json
import requests


def check_user_exists(userId):
    return bool(json.loads(requests.get(f"https://localhost:44381/CheckUserExists/{userId}", verify=False).text))


def check_user_is_sponsor(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckUserIsSponsor/{userId}", verify=False).text))


def check_user_is_awaiting(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckUserIsAwaiting/{userId}", verify=False).text))


def check_user_is_awaiting_by_username(username):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckUserIsAwaitingByUsername/{username}", verify=False).text))


def check_sponsor_is_maxed(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckSponsorIsMaxed/{userId}", verify=False).text))


def check_user_keyword_is_correct(userId, keyword):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckUserKeyWordIsCorrect/{userId}/{keyword}", verify=False).text))


def check_user_is_admin(userId):
    return bool(
        json.loads(requests.get(f"https://localhost:44381/CheckUserIsAdmin/{userId}", verify=False).text))


def check_user_is_postponed(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckSponsorIsPostponed/{userId}", verify=False).text))


def check_user_is_registered(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckUserIsRegistered/{userId}", verify=False).text))


def check_user_has_visited_section(userId, sectionId):
    try:
        return bool(json.loads(
            requests.get(f"https://localhost:44381/CheckUserHasVisitedSection/{userId}/{sectionId}",
                         verify=False).text))
    except:
        return None


def check_user_is_banned(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckUserIsBanned/{userId}", verify=False).text))


def check_user_is_busy(userId):
    try:
        return bool(json.loads(
            requests.get(f"https://localhost:44381/CheckUserIsBusy/{userId}", verify=False).text))
    except:
        return None


def switch_user_busy_status(userId):
    try:
        return requests.get(f"https://localhost:44381/SwhitchUserBusyStatus/{userId}", verify=False).text
    except:
        return None


def check_user_is_deleted(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckUserIsDeleted/{userId}", verify=False).text))


def check_users_are_combinable(user1, user2):
    return bool(
        json.loads(
            requests.get(f"https://localhost:44381/CheckUsersAreCombinableRT/{user1}/{user2}", verify=False).text))


def get_common_langs(user1Id, user2Id, localisationId):
    return requests.get(f"https://localhost:44381/RetreiveCommonLanguages/{user1Id}/{user2Id}/{localisationId}",
                        verify=False).text


def check_user_has_requests(userId):
    try:
        return bool(
            json.loads(requests.get(f"https://localhost:44381/CheckUserHasRequests/{userId}", verify=False).text))
    except:
        return None


def check_user_has_notifications(userId):
    try:
        return bool(
            json.loads(requests.get(f"https://localhost:44381/CheckUserHasNotifications/{userId}", verify=False).text))
    except:
        return None


def check_user_has_premium(userId):
    try:
        return bool(
            json.loads(requests.get(f"https://localhost:44381/CheckUserHasPremium/{userId}", verify=False).text))
    except:
        return None


def check_user_uses_personality(userId):
    try:
        return bool(
            json.loads(requests.get(f"https://localhost:44381/CheckUserUsesPersonality/{userId}", verify=False).text))
    except:
        return None


def check_user_in_a_blacklist(userId, encounteredUser):
    try:
        return bool(
            json.loads(
                requests.get(f"https://localhost:44381/CheckEncounteredUserIsInBlackList/{userId}/{encounteredUser}",
                             verify=False).text))
    except:
        return None


def get_user_language_limit(userId):
    try:
        return int(
            json.loads(
                requests.get(f"https://localhost:44381/GetUserMaximumLanguageCount/{userId}", verify=False).text))
    except:
        return None


def get_user_limitations(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/limitations/{userId}", verify=False).text)
    except:
        return None


def get_user_basic_info(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/basic-info/{userId}", verify=False).text)
    except:
        return None


def get_user_app_language(userId):
    try:
        return int(
            json.loads(requests.get(f"https://localhost:44381/GetUserAppLanguage/{userId}", verify=False).text))
    except:
        return None


def get_user_active_reply(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetActiveAutoReply/{userId}", verify=False).text)
    except:
        return None


def get_user_request(requestId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/CheckUserHasRequest/{requestId}", verify=False).text)
    except:
        return None


def get_user_info(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/UserInfo/{userId}", verify=False).text)
    except:
        return None


def get_sponsor_info(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetSponsorInfo/{userId}", verify=False).text)
    except:
        return None


def get_sponsor_languages(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetSponsorLanguages/{userId}", verify=False).text)
    except:
        return None


def get_user_base_info(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetUserBaseInfo/{userId}", verify=False).text)
    except:
        return None


def get_user_requests(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetUserRequests/{userId}", verify=False).text)
    except:
        return None


def get_user_notifications(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetUserNotifications/{userId}", verify=False).text)
    except:
        return None


def delete_user_requests(userId):
    try:
        return json.loads(requests.delete(f"https://localhost:44381/DeleteUserRequests/{userId}", verify=False).text)
    except:
        return None


def delete_user_request(requestId):
    try:
        return json.loads(requests.delete(f"https://localhost:44381/DeleteUserRequest/{requestId}", verify=False).text)
    except:
        return None


def delete_user_notification(notificationId):
    try:
        return bool(
            json.loads(requests.get(f"https://localhost:44381/SendNotificationConfirmationCode/{notificationId}",
                                    verify=False).text))
    except:
        return None


def start_program_in_debug_mode(bot):  # TODO: remove in production
    requests.get("https://localhost:44381/SetDebugProperties", verify=False)
    return json.loads(requests.get("https://localhost:44381/GetAllUsersIds", verify=False).text)


def get_request_sender(requestId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetRequestSender/{requestId}", verify=False).text)
    except:
        return None


def get_user_list(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetUserList/{userId}", verify=False).text)
    except:
        return None


def get_user_list_turnOffPersonality(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381//GetUserList/TurnOffP/{userId}", verify=False).text)
    except:
        return None


def get_free_user_list(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381//GetUserList/FreeSearch/{userId}", verify=False).text)
    except:
        return None


def get_user_list_by_tags(getUserByTagsModel):
    try:
        d = json.dumps(getUserByTagsModel)
        return json.loads(requests.post(f"https://localhost:44381/GetUserByTags", d,
                                        headers={"Content-Type": "application/json"},
                                        verify=False).text)
    except:
        return None


def user_invitation_link(invitationId, userId):
    try:
        return bool(
            json.loads(requests.get(f"https://localhost:44381/InviteUser/{invitationId}/{userId}", verify=False).text))
    except:
        return None


def register_user_request(senderId, receiverId, isLikedBack, description=""):
    try:
        data = {
            "userId": senderId,
            "userId1": receiverId,
            "isLikedBack": isLikedBack,
            "description": description
        }

        d = json.dumps(data)
        return requests.post(f"https://localhost:44381/RegisterUserRequest", d,
                             headers={"Content-Type": "application/json"},
                             verify=False).text
    except:
        return None


def decline_user_request(user1, user2):
    try:
        return requests.get(f"https://localhost:44381/DeclineRequest/{user1}/{user2}",
                            verify=False).text
    except:
        return None


def register_user_encounter(current_user_id, user_id, section_id):
    try:
        data = {
            "id": 0,
            "userId": current_user_id,
            "userId1": user_id,
            "sectionId": section_id,
        }

        d = json.dumps(data)

        de = requests.post("https://localhost:44381/RegisterUserEncounter", d,
                           headers={"Content-Type": "application/json"},
                           verify=False).text
        return json.loads(de)
    except:
        return None


def register_user_encounter_familiator(current_user_id, user_id):
    try:
        register_user_encounter(current_user_id, user_id, 2)
    except:
        return None


def register_user_encounter_rt(current_user_id, user_id):
    try:
        register_user_encounter(current_user_id, user_id, 5)
    except:
        return None


def get_all_user_achievements(userId):
    return json.loads(requests.get(f"https://localhost:44381/GetUserAchievements/{userId}", verify=False).text)


def get_all_user_achievements_admin(userId):
    return json.loads(requests.get(f"https://localhost:44381/GetUserAchievementsAsAdmin/{userId}", verify=False).text)


def get_active_user_balance(userId):
    try:
        return json.loads(
            requests.get(f"https://localhost:44381/GetActiveUserWalletBalance/{userId}", verify=False).text)
    except:
        return None


def top_up_user_balance(userId, amount, description):
    return json.loads(requests.get(f"https://localhost:44381/TopUpUserWalletBalance/{userId}/{amount}/{description}",
                                   verify=False).text)


def check_user_balance_is_sufficient(userId, cost):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckBalanceIsSufficient/{userId}/{cost}", verify=False).text))


def check_user_have_chosen_free_search(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckUserHaveChosenFreeParam/{userId}", verify=False).text))


def check_should_turnOf_personality(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckShouldTurnOffPersonality/{userId}", verify=False).text))


def check_promo_is_valid(userId, promo, isActivatedBeforeRegistration):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckPromoIsCorrect/{userId}/{promo}/{isActivatedBeforeRegistration}",
                     verify=False).text))


def grant_premium_for_points(userId, cost, dayDuration):
    return json.loads(requests.get(f"https://localhost:44381/GrantPremiumToUser/{userId}/{cost}/{dayDuration}/{1}",
                                   verify=False).text)


def grant_premium_for_real_money(userId, cost, dayDuration):
    return json.loads(requests.get(f"https://localhost:44381/GrantPremiumToUser/{userId}/{cost}/{dayDuration}/{4}",
                                   verify=False).text)


def purchase_effect_for_points(userId, effectId, cost, count=1):
    return json.loads(
        requests.get(f"https://localhost:44381/PurchaseEffect/{userId}/{effectId}/{cost}/1/{count}", verify=False).text)


def check_user_has_effect(userId, effectId):
    return bool(
        json.loads(requests.get(f"https://localhost:44381/CheckUserHasEffect/{userId}/{effectId}", verify=False).text))


# TODO: Change called API endpoint
def purchase_effect_for_real_money(userId, effectId, cost):
    return json.loads(
        requests.get(f"https://localhost:44381/GrantPremiumToUser/{userId}/{effectId}/{cost}/{4}", verify=False).text)


def purchase_PP_for_points(userId, cost, count=1):
    return json.loads(
        requests.get(f"https://localhost:44381/PurchesPPForPoints/{userId}/{cost}/{count}", verify=False).text)


def switch_admin_status(userId):
    admin_switch_result = {
        0: "Sorry, you were not authorized as an admin",
        1: "Done. Your status had been changed"
    }

    return admin_switch_result[int(json.loads(requests.get(f"https://localhost:44381/SwitchAdminStatus/{userId}",
                                                           verify=False).text))]


def switch_personality_status(userId):
    return requests.get(f"https://localhost:44381/SwitchPersonalityUsage/{userId}",
                        verify=False)


def switch_familiarity_status(userId):
    return requests.get(f"https://localhost:44381/SwitchIncreasedFamiliarity/{userId}",
                        verify=False)


def switch_hint_status(userId):
    return requests.get(f"https://localhost:44381/set-hint-status/{userId}",
                        verify=False)


def switch_comment_status(userId):
    return requests.get(f"https://localhost:44381/set-comment-status/{userId}",
                        verify=False)


def get_increased_familiarity_status(userId):
    return bool(json.loads(requests.get(f"https://localhost:44381/GetUserIncreasedFamiliarity/{userId}",
                                        verify=False).text))


def update_user_status(userId, status):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/UpdateUserNickname/{userId}/{status}", verify=False).text))


def get_admin_status(userId):
    data = {
        False: "Disabled",
        True: "Enabled"
    }

    d = requests.get(f"https://localhost:44381/GethAdminStatus/{userId}",
                     verify=False)
    if d.text:
        return data[bool(json.loads(d.text))]
    return None


def set_user_currency(userId, currency):
    response = requests.get(f"https://localhost:44381/SetUserCurrency/{userId}/{currency}",
                            verify=False)

    if 100 < response.status_code < 300:
        return True

    return False


def register_adventure(adventureData):
    try:
        d = json.dumps(adventureData)
        return requests.post(f"https://localhost:44381/RegisterAdventure", d,
                             headers={"Content-Type": "application/json"},
                             verify=False).text
    except:
        return None


def change_adventure(adventureData):
    try:
        d = json.dumps(adventureData)
        return requests.post(f"https://localhost:44381/ChangeAdventure", d,
                             headers={"Content-Type": "application/json"},
                             verify=False).status_code
    except:
        return None


def save_template(adventureData):
    try:
        d = json.dumps(adventureData)
        return requests.post(f"https://localhost:44381/SaveTemplate", d,
                             headers={"Content-Type": "application/json"},
                             verify=False).status_code
    except:
        return None
