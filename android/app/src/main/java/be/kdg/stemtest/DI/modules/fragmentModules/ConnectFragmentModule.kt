package be.kdg.stemtest.DI.modules.fragmentModules

import be.kdg.stemtest.view.fragments.Connect
import dagger.Module
import dagger.android.ContributesAndroidInjector

@Module
internal abstract class ConnectFragmentModule {

        @ContributesAndroidInjector
        abstract fun bindConnectFragment(): Connect
    }