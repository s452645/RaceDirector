class AbstractTimer:
    def get_current_time(self):
        raise NotImplementedError(
            "This is an abstract method, it must be implemented in a subclass"
        )
