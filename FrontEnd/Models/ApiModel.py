import json


class ApiModel:
    def to_json(self):
        data = {key: getattr(self, key) for key in self.__dict__}
        return json.dumps(data)
