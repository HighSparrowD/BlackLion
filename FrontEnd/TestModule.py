import Core.HelpersMethodes as Helpers


class TestModule:
    def __init__(self, bot, message):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        Helpers.switch_user_busy_status(self.current_user)