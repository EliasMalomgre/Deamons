package be.kdg.stemtest.DI.base

import android.app.Application
import be.kdg.stemtest.DI.DaggerApplicationComponent
//import be.kdg.stemtest.DI.DaggerApplicationComponent
import dagger.android.*
import javax.inject.Inject


class BaseApplication  : Application(),HasAndroidInjector {

        @Inject
        lateinit var dispatchingAndroidInjector: DispatchingAndroidInjector<Any>


        override fun onCreate() {
            super.onCreate()
            DaggerApplicationComponent.builder().application(this).build()
                .inject(this)
        }


     override fun androidInjector(): AndroidInjector<Any>? {
            return dispatchingAndroidInjector
        }
    }