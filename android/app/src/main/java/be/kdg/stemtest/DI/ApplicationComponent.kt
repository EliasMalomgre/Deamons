package be.kdg.stemtest.DI


import be.kdg.stemtest.DI.modules.*
import be.kdg.stemtest.DI.base.BaseApplication
import dagger.BindsInstance
import dagger.Component
import dagger.android.support.AndroidSupportInjectionModule
import javax.inject.Singleton

@Singleton
@Component(modules = [
    NetworkModule::class,
    DatabaseModule::class,
    AppModule::class,
    BindModule::class,
    ViewModelModule::class,
    AndroidSupportInjectionModule::class,
    MainActivityModule::class
])

interface ApplicationComponent {
    fun inject(application: BaseApplication)


    @Component.Builder
    interface Builder{
        @BindsInstance
        fun application(application: BaseApplication): Builder
        fun build():ApplicationComponent
    }
}