from abc import ABC, abstractmethod
class IAffectingClass(ABC):
    _static_property = 0


    @property
    def val(self):
        return self._val


    @val.setter
    def val(self, value):
        self._val = value


    @property
    @staticmethod
    def static_property():
        return IAffectingClass._static_property


    @staticmethod
    def _set_static_property(value):
        IAffectingClass._static_property = value


    @abstractmethod
    def method(self):
        pass


    @staticmethod
    def static_dependency():
        return 2


    def __private_method(self):
        return 0




class AffectingClass(IAffectingClass):
    def __init__(self):
        self._val = 0


    def method(self):
        return 1




class ClassUnderTest:
    def __init__(self, p_aff):
        self._i_aff = p_aff


    def public_method(self, arg):
        return self.__private_method(arg)


    def _protected_method(self, arg):
        if arg != 0:
            return self._i_aff.val
        else:
            raise ValueError("arg is equal to zero")


    def __private_method(self, arg):
        return self._i_aff.val if self._i_aff.val > arg else arg


    def call_static(self):
        return IAffectingClass.static_dependency()




class ClassUnderTest2:
    def __init__(self, p_aff_instance):
        self.aff_instance = p_aff_instance


    def call_affecting_method(self):
        return self.aff_instance.method()




if __name__ == "__main__":
    pass
