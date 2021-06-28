package be.kdg.stemtest.DI.modules

import android.app.Application
import be.kdg.stemtest.DI.base.BaseApplication
import dagger.Binds
import dagger.Module

@Module
abstract class BindModule {
    @Binds
    abstract fun bindApplication(baseApplication: BaseApplication) : Application
}