package be.kdg.stemtest.DI.modules

import android.app.Application
import android.content.Context
import dagger.Module
import dagger.Provides

@Module
class AppModule {

    @Provides
    fun ProvideContext(application: Application): Context{
        return application
    }



}