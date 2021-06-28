package be.kdg.stemtest.DI.modules.fragmentModules

import be.kdg.stemtest.view.fragments.customdebategame.CDgResult
import dagger.Module
import dagger.android.ContributesAndroidInjector

@Module
abstract class CDgResultFragmentModule {

    @ContributesAndroidInjector
    abstract fun bindCDgResultFragment(): CDgResult
}