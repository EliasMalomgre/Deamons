package be.kdg.stemtest.DI.modules.fragmentModules

import be.kdg.stemtest.view.fragments.debategame.DgResult
import dagger.Module
import dagger.android.ContributesAndroidInjector

@Module
abstract class DgResultFragmentModule {

    @ContributesAndroidInjector
    abstract fun bindConnectFragment(): DgResult
}